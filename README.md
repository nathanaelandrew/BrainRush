# BrainRush

Timed trivia battles: movies, music, sports, gaming and general knowledge, with streaks, a leaderboard and daily challenges with friends.

**Stack:** Blazor WebAssembly (.NET 8) · Supabase (Postgres + auto-generated REST API) · Tailwind CSS

## Run it

1. **Create the Supabase project.** On [supabase.com](https://supabase.com), create a new project (Free plan, region *Southeast Asia (Singapore)*).
2. **Create the tables.** Go to Dashboard → **SQL Editor** → **New query**, paste [`database/schema.sql`](database/schema.sql) and click **Run**. Then do the same with [`database/seed.sql`](database/seed.sql) to load the starter rows.
3. **Copy your keys.** Click **Connect** and copy the **Project URL**. Then go to Settings → **API Keys** and copy the **Publishable key**. Leave the secret key alone.
4. **Paste them into [`wwwroot/appsettings.json`](wwwroot/appsettings.json):**
   ```json
   { "Supabase": { "Url": "https://YOUR-REF.supabase.co", "Key": "sb_publishable_..." } }
   ```
5. **Run:**
   ```bash
   dotnet restore
   dotnet run
   ```
   This opens http://localhost:5246.

Optional quick test before running the app (paste into a `.http` file in VS Code or Rider):
```http
GET https://YOUR-REF.supabase.co/rest/v1/leaderboard?select=name,total&order=total.desc
apikey: sb_publishable_...
```

If you change any Tailwind classes, rebuild the CSS with `npm install` and then `npm run build:css`.

## Database

| Table | What it holds | Keys and rules |
|---|---|---|
| `players` | Each person, plus their current and best streak | `handle` unique; `best_streak >= streak`; at most one row with `is_you = true` |
| `questions` | The question bank | `topic` is the `category` enum; exactly 4 options; `correct_index` 0–3; `seconds` 10–30 |
| `challenges` | Daily challenges | `host_id` → `players`; title 3–40 characters; 3–10 questions; note ≤ 120 characters |
| `challenge_players` | Who is invited, and when they finished | Primary key `(challenge_id, player_id)`, both foreign keys |
| `rounds` | One row per finished round | `player_id` → `players`; `correct <= total` |

The **views** (`leaderboard`, `question_pool`, `question_counts`, `challenge_cards`, `site_stats`) do the joining, summing and filtering inside Postgres, so each page needs one request per list instead of a fetch inside a loop.

### Fast habits
- **Filter in the database.** Every `.Where()`, `.Or()`, `.Order()` and `.Range()` call comes before `.Get()`. The Leaderboard's Friends tab, search box, category and sort, plus the Question bank's arena filter, all run as SQL. The browser never downloads rows only to throw them away.
- **Indexes match the filters:** `questions (topic) where deleted_at is null`, `challenges (created_at desc) where deleted_at is null`, and indexes on every foreign key.
- **No fetch inside a loop.** Sending a challenge to several friends is one `INSERT` with a list. The leaderboard is one `GET` from a view, not one request per player. Home sends its four independent requests at the same time.
- **Double-click safe.** Save buttons are disabled while their request is running, so a double click cannot send two POSTs.

## Storage pattern and why

**`questions` and `challenges` use soft delete; `rounds` is a history table.**

Deleting a question or cancelling a challenge only fills in `deleted_at`, and the pages and views skip any row where it is set. Questions are reused by every round and every challenge, so a mistaken delete should be easy to undo; the Question bank has a **Deleted** view with a **Restore** button that simply clears `deleted_at`. A cancelled challenge keeps its record of who was invited and who had already played, rather than leaving those players with an invitation that points nowhere. `rounds`, by contrast, is never updated or deleted. The leaderboard totals and accuracy are summed from it, so its history must stay exact. The database enforces both rules itself: `anon` has no `DELETE` grant on the soft-delete tables, and only `SELECT` and `INSERT` on `rounds`. `players.streak` is a plain overwrite, because only the current value matters.

## What each page does

| Page | Read | Create / update / delete |
|---|---|---|
| Home | Stats, arena counts, top three, featured challenge | — |
| Play | Questions for the round (random, filtered by topic) | **Insert** a `rounds` row, **update** your streak, **update** `finished_at` on a challenge |
| Leaderboard | Ranked players, filtered and sorted in the database | — |
| Daily challenge | Friends and live challenges | **Insert** a challenge and its invites; **soft delete** (cancel) your own |
| Question bank | Live or deleted questions, filtered by arena | **Insert**, **update**, **soft delete** and **restore** questions |

> The open `anon` policies in `schema.sql` are for class only. A real app would add login and check `auth.uid()` in each policy.
