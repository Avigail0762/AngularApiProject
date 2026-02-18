# AI Coding Agent Instructions

These notes make agents productive quickly in this Angular 20 standalone app.

## Architecture
- Root app boots via [src/main.ts](src/main.ts) and providers in [src/app/app.config.ts](src/app/app.config.ts) (`provideRouter(routes)`, `provideHttpClient()`, global error listeners, zone change detection).
- `App` component is the shell: see [src/app/app.ts](src/app/app.ts). It imports feature components directly (standalone) and reads a JWT from `localStorage`, decodes it with `jwt-decode`, and exposes `Role` for UI gating.
- Routing is currently empty in [src/app/app.routes.ts](src/app/app.routes.ts). Navigation is mostly component-driven (no configured routes yet) despite `RouterOutlet` being present in `App`.
- Feature areas are grouped by domain under [src/app/components](src/app/components): `Donor`, `Gift`, `lottery`, `auth`. Each component is standalone and declares needed Angular modules in `imports`.
- HTTP services live in [src/app/services](src/app/services): `donor-service.ts`, `gifts-service.ts`, `lottery-service.ts`, `auth-service.ts`. They encapsulate backend calls to `https://localhost:7253/api/*`.
- Models vs DTOs: domain classes are in [src/app/models](src/app/models), and wire-format DTOs in [src/app/models/Dto](src/app/models/Dto).

## Key Patterns
- Standalone components: use `imports: [CommonModule, FormsModule/ReactiveFormsModule, ...]` and `styleUrl`/`templateUrl` (see [Donors component](src/app/components/Donor/donors/donors.ts)). Prefer `inject(Service)` over constructor DI.
- Observable data flow: list pages keep a `$`-suffixed observable (e.g., `donors$`, `gifts$`) populated via service methods and refresh after mutations by reassigning to `getAll()`.
- Auth token: services build an `Authorization: Bearer <token>` header from `localStorage['authToken']` via a common `getAuthHeader()` (e.g., [donor-service](src/app/services/donor-service.ts)). Missing token triggers a Hebrew alert and returns `undefined`; call sites should avoid firing secured requests without a token.
- Login flow: [auth-service](src/app/services/auth-service.ts) posts credentials, saves `authToken` on success, uses simple `alert` messaging; no router navigation implemented yet.
- Role usage: [App](src/app/app.ts) decodes the token, sets `localStorage['decoded']` to `decoded.Role`, and exposes `Role` for conditional rendering; there is no centralized guard.

## Developer Workflows
- Start dev server: `npm start` (alias for `ng serve`). Config is in [angular.json](angular.json) under `serve` with default `development`.
- Build: `npm run build` (production by default; SCSS and assets from [public](public) per [angular.json](angular.json)).
- Unit tests: `npm test` (Karma/Jasmine). Specs co-located next to sources (e.g., services/components).
- Code scaffolding: use Angular CLI (see [README.md](README.md)); styles default to SCSS via schematics.

## Integration Details
- Backend base URLs:
  - Donors: `https://localhost:7253/api/donor` ([donor-service](src/app/services/donor-service.ts))
  - Gifts: `https://localhost:7253/api/gift` ([gifts-service](src/app/services/gifts-service.ts))
  - Lottery: `https://localhost:7253/api/lottery` ([lottery-service](src/app/services/lottery-service.ts))
  - Auth: `https://localhost:7253/api/auth` ([auth-service](src/app/services/auth-service.ts))
- Typical CRUD shape: `getAll()`, `getById()`/`geByName()`, `add(dto)`, `update(dto, id)`, `delete(id)`; consumers subscribe for side effects and refresh observables.
- External libs: `jwt-decode` for token parsing; RxJS for async; Angular 20 standalone APIs.

## Conventions & Examples
- Place feature components under domain folders; name the class to match the folder (e.g., `Donors` in [Donors component](src/app/components/Donor/donors/donors.ts)).
- Use DTOs for payloads (e.g., `DonorDTO`, `GiftDTO`) and models for UI state (e.g., `Donor`, `Gift`).
- Add new secured service calls by following `getAuthHeader()` and base URL pattern in existing services.
- If adding routes, update [src/app/app.routes.ts](src/app/app.routes.ts) and keep standalone components lazy if needed; wire with `provideRouter(routes)` in [src/app/app.config.ts](src/app/app.config.ts).

## Gotchas
- Missing auth token yields alerts and `undefined` headers; ensure login succeeds before invoking secured endpoints.
- Lottery `draw(giftId)` posts to `/draw/<giftId>` and also sends `giftId` in the body (see [lottery-service](src/app/services/lottery-service.ts)). Align with backend expectations.
- `RouterOutlet` exists but routes are empty; navigation won’t work until routes are defined.

Questions or gaps? Point me to areas needing deeper guidance (e.g., intended route structure, auth guards, post-login navigation), and I’ll iterate this doc.