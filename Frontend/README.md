# Campus Services Portal — Angular Frontend

Production-style Angular 21 standalone frontend built against the supplied ASP.NET Core backend and BRD.

## What is implemented

- JWT login, registration, password reset request/reset, sessionStorage token persistence and role guards.
- Role-aware responsive application shell, dashboards and navigation.
- Student: profile, hostel applications, lab slot/seat reservations, event registrations/reserved seats, complaints, certificate requests, fees/payment simulation/printable receipt, notifications.
- Administrator: students, student-master CSV import, faculties, admin accounts, hostels/rooms/applications, labs/seats/time slots/bookings, events/venues/seats/registrations, complaints/categories, certificate types/requests, fees/types/assignment/status, system settings.
- Reusable loading/empty/status/stat/toast UI patterns, responsive tables/cards, accessible labels, confirmation prompts and 409-friendly error messages.

## Backend integration

The development API URL is in `src/environments/environment.ts`:

```ts
apiUrl: 'http://localhost:5280/api'
```

The supplied backend launch profile exposes HTTPS on `https://localhost:7281` and HTTP on `http://localhost:5280`. Development uses the HTTP profile by default so `dotnet run --launch-profile http` and `npm start` work together without a local certificate prompt. Production builds replace this with the same-origin `/api` URL.

The backend CORS policy must allow `http://localhost:4200` (the supplied backend already defaults to that origin).

## Run

```bash
npm install
npm start
```

Open `http://localhost:4200`.

Production build:

```bash
npm run build:prod
```

## Important integration note

The supplied lab backend provides `GET /api/labs/{labId}/slots?date=...` but does **not** expose a separate "list all configured time slots for lab" endpoint. Therefore the admin lab screen manages/edits slot IDs returned for the selected inspection date, while creation is available directly. Everything else is wired to the concrete controllers/DTOs in the supplied backend.

## Project structure

- `core/models` — typed API/domain contracts
- `core/services` — one service per domain area, all HttpClient access centralized
- `core/guards` — authentication and role guards
- `core/interceptors` — JWT Authorization header
- `shared` — reusable UI components/pipes
- `features` — lazy-loaded screens for each functional module
- `layout` — responsive student/admin shell

`node_modules` is intentionally not included in the ZIP.
