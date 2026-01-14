Neksara — learning content platform
===================================

Short: an ASP.NET Core MVC site for publishing learning Topics grouped into Categories, with admin CRUD, archive/restore, and simple caching for user pages.

Quick start (local)
-------------------
Prerequisites:
- .NET 8 SDK
- SQL Server (or update connection string to a local DB)

1. Configure connection string in `appsettings.json` (key `DefaultConnection`).
2. Build and run with hot reload:

```bash
dotnet build
dotnet watch
```

3. Seed an admin user (optional): the project seeds a default admin if `Users` empty, but you can insert one manually:

```sql
INSERT INTO Users (Username, Email, Password, Role, CreatedAt)
VALUES ('admin', 'admin@example.com', 'admin123', 'Admin', GETDATE());
```

Important workflows for demo
----------------------------
- Admin → Create Category + Create 2 Topics (one topic may include a `VideoUrl` — either a YouTube URL or a full `<iframe>` embed for providers like Wordwall).
- Admin → Delete Category: archives the category and its topics into `ArchiveCategories` / `ArchiveTopics` (snapshots saved).
- Admin → Archive page: view archived categories and topics separately.
- Admin → Restore Topic: restores the topic and undeletes/creates its category; archived category entry is removed (Option A behavior).

Caching behaviour
-----------------
- Topic lists (user-facing `Learning/Topics`) are cached per category/page for 60 seconds. Key format: `TopicList_{category}_page_{n}` (`Controllers/LearningController.cs`).
- Homepage popular topics cached 60s (key `PopularTopics_take_{n}` in `Controllers/HomeController.cs`).
- When a topic detail is opened, `IncrementViewCountAsync` increases the view count and invalidates the homepage popular cache so changes appear immediately (`Services/LearningService.cs`).

Video embeds and admin iframe support
-----------------------------------
- Admin create/edit now accepts either a URL or a full `<iframe>` embed in the `VideoUrl` field (textarea). See `Views/Admin/CreateTopic.cshtml` and `Views/Admin/EditTopic.cshtml`.
- Server sanitizes the input by stripping `<script>` tags and `javascript:` URIs before saving (`Services/TopicService.cs`). The detail view (`Views/Learning/Detail.cshtml`) will render raw iframe HTML if the stored value begins with `<iframe`.
- Note: some providers (Wordwall) may still block framing (X-Frame-Options/CSP). In that case open the resource in a new tab.

Database & migrations
---------------------
- Migrations are under the `Migrations/` folder. Recent important migration: `MakeArchiveIndependent` — it removed a restrictive FK between `ArchiveTopics` and `Category`, made `ArchiveTopics.CategoryId` nullable and added `CategoryName` snapshot.
- The app runs `context.Database.Migrate()` on startup (`Program.cs`).
