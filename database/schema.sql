-- =====================================================================
-- BrainRush · database/schema.sql
-- Run in Supabase: Dashboard → SQL Editor → New query → paste → Run.
-- Then run database/seed.sql to load the starter rows.
--
-- Storage patterns used here:
--   questions, challenges → SOFT DELETE  (deleted_at is filled in, row stays)
--   rounds                → HISTORY TABLE (insert-only, never updated)
--   players               → OVERWRITE   (streak is replaced after each round)
-- The open anon policies below are for class only.
-- =====================================================================

-- One list of topics, shared by every table that needs one.
create type public.category as enum ('Movies', 'Music', 'Sports', 'Gaming', 'General');

-- ---------------------------------------------------------------------
-- players · one row per person. "is_you" stands in for a login for now.
-- ---------------------------------------------------------------------
create table public.players (
  id          bigint generated always as identity primary key,
  name        text    not null check (char_length(name) between 1 and 60),
  handle      text    not null unique check (handle ~ '^[a-z0-9_]{2,20}$'),
  is_friend   boolean not null default false,
  is_you      boolean not null default false,
  streak      int     not null default 0 check (streak >= 0),
  best_streak int     not null default 0 check (best_streak >= streak),
  created_at  timestamptz not null default now()
);
-- At most one row can be "you".
create unique index players_only_one_you on public.players (is_you) where is_you;

-- ---------------------------------------------------------------------
-- questions · the question bank. Soft delete keeps old rows.
-- ---------------------------------------------------------------------
create table public.questions (
  id            bigint generated always as identity primary key,
  topic         public.category not null,
  prompt        text     not null check (char_length(prompt) between 10 and 200),
  options       text[]   not null check (array_length(options, 1) = 4 and not ('' = any (options))),
  correct_index smallint not null check (correct_index between 0 and 3),
  seconds       smallint not null default 15  check (seconds between 10 and 30),
  points        smallint not null default 100 check (points > 0),
  created_at    timestamptz not null default now(),
  deleted_at    timestamptz                                   -- soft delete
);
-- Habit 1: every page filters on topic + "not deleted", so index exactly that.
create index questions_live_topic on public.questions (topic) where deleted_at is null;

-- ---------------------------------------------------------------------
-- challenges · a daily challenge a player sends to friends. Soft delete = cancel.
-- ---------------------------------------------------------------------
create table public.challenges (
  id             bigint generated always as identity primary key,
  title          text     not null check (char_length(title) between 3 and 40),
  topic          public.category not null,
  host_id        bigint   not null references public.players (id),
  question_count smallint not null default 5  check (question_count between 3 and 10),
  seconds        smallint not null default 15 check (seconds between 10 and 30),
  note           text     check (char_length(note) <= 120),
  created_at     timestamptz not null default now(),
  deleted_at     timestamptz                                  -- soft delete
);
create index challenges_live_newest on public.challenges (created_at desc) where deleted_at is null;
create index challenges_host on public.challenges (host_id);

-- ---------------------------------------------------------------------
-- challenge_players · who was invited to which challenge, and who finished.
-- ---------------------------------------------------------------------
create table public.challenge_players (
  challenge_id bigint not null references public.challenges (id) on delete cascade,
  player_id    bigint not null references public.players (id),
  invited_at   timestamptz not null default now(),
  finished_at  timestamptz,
  primary key (challenge_id, player_id)
);
create index challenge_players_player on public.challenge_players (player_id);

-- ---------------------------------------------------------------------
-- rounds · HISTORY TABLE. One row per finished round, never changed.
-- The leaderboard is computed from these rows.
-- ---------------------------------------------------------------------
create table public.rounds (
  id           bigint generated always as identity primary key,
  player_id    bigint   not null references public.players (id),
  topic        public.category,                               -- null = mixed round
  challenge_id bigint   references public.challenges (id),
  score        int      not null check (score >= 0),
  correct      smallint not null check (correct >= 0),
  total        smallint not null check (total > 0 and correct <= total),
  best_streak  smallint not null default 0 check (best_streak between 0 and total),
  played_at    timestamptz not null default now()
);
create index rounds_player on public.rounds (player_id);

-- =====================================================================
-- Views · let the database do the joining, filtering and summing,
-- so each page needs one request instead of a fetch inside a loop.
-- security_invoker = true makes the views obey the tables' RLS policies.
-- =====================================================================

-- One row per player with points per category, total and accuracy.
create view public.leaderboard with (security_invoker = true) as
select
  p.id, p.name, p.handle, p.is_friend, p.is_you, p.streak, p.best_streak,
  (p.is_friend or p.is_you) as in_circle,                     -- "Friends" tab
  coalesce(round(100.0 * sum(r.correct) / nullif(sum(r.total), 0)), 0)::int as accuracy,
  coalesce(sum(r.score) filter (where r.topic = 'Movies'),  0)::int as movies,
  coalesce(sum(r.score) filter (where r.topic = 'Music'),   0)::int as music,
  coalesce(sum(r.score) filter (where r.topic = 'Sports'),  0)::int as sports,
  coalesce(sum(r.score) filter (where r.topic = 'Gaming'),  0)::int as gaming,
  coalesce(sum(r.score) filter (where r.topic = 'General'), 0)::int as general,
  coalesce(sum(r.score), 0)::int as total
from public.players p
left join public.rounds r on r.player_id = p.id
group by p.id;

-- Live questions in random order. Filter by topic and take N in one request.
create view public.question_pool with (security_invoker = true) as
select id, topic, prompt, options, correct_index, seconds, points
from public.questions
where deleted_at is null
order by random();

-- How many live questions each topic has.
create view public.question_counts with (security_invoker = true) as
select topic, count(*)::int as questions
from public.questions
where deleted_at is null
group by topic;

-- Live challenges with host and invited players folded into one row.
create view public.challenge_cards with (security_invoker = true) as
select
  c.id, c.title, c.topic, c.question_count, c.seconds, c.note, c.created_at,
  c.host_id, h.name as host_name, h.handle as host_handle,
  coalesce(
    jsonb_agg(
      jsonb_build_object('id', p.id, 'name', p.name, 'handle', p.handle,
                         'finished', cp.finished_at is not null)
      order by cp.finished_at nulls last, cp.invited_at, p.id
    ) filter (where p.id is not null),
    '[]'::jsonb
  ) as members
from public.challenges c
join public.players h on h.id = c.host_id
left join public.challenge_players cp on cp.challenge_id = c.id
left join public.players p on p.id = cp.player_id
where c.deleted_at is null
group by c.id, h.id;

-- Headline numbers for the home page, in a single row.
create view public.site_stats with (security_invoker = true) as
select
  (select count(*) from public.players)::int                                   as players,
  (select coalesce(max(best_streak), 0) from public.players)::int              as best_streak,
  (select count(*) from public.questions  where deleted_at is null)::int       as questions,
  (select count(*) from public.challenges where deleted_at is null)::int       as open_challenges;

-- =====================================================================
-- Locks · lock 1 = GRANT (which verbs anon may send),
--         lock 2 = row level security policies.
-- Soft-delete tables get no DELETE at all; rounds gets no UPDATE/DELETE.
-- =====================================================================
revoke all on public.players, public.questions, public.challenges,
              public.challenge_players, public.rounds from anon;

grant select, update         on public.players           to anon;
grant select, insert, update on public.questions         to anon;
grant select, insert, update on public.challenges        to anon;
grant select, insert, update on public.challenge_players to anon;
grant select, insert         on public.rounds            to anon;
grant select on public.leaderboard, public.question_pool, public.question_counts,
                public.challenge_cards, public.site_stats to anon;

alter table public.players           enable row level security;
alter table public.questions         enable row level security;
alter table public.challenges        enable row level security;
alter table public.challenge_players enable row level security;
alter table public.rounds            enable row level security;

create policy "read_all"   on public.players for select to anon using (true);
create policy "update_all" on public.players for update to anon using (true) with check (true);

create policy "read_all"   on public.questions for select to anon using (true);
create policy "insert_all" on public.questions for insert to anon with check (true);
create policy "update_all" on public.questions for update to anon using (true) with check (true);

create policy "read_all"   on public.challenges for select to anon using (true);
create policy "insert_all" on public.challenges for insert to anon with check (true);
create policy "update_all" on public.challenges for update to anon using (true) with check (true);

create policy "read_all"   on public.challenge_players for select to anon using (true);
create policy "insert_all" on public.challenge_players for insert to anon with check (true);
create policy "update_all" on public.challenge_players for update to anon using (true) with check (true);

create policy "read_all"   on public.rounds for select to anon using (true);
create policy "insert_all" on public.rounds for insert to anon with check (true);
