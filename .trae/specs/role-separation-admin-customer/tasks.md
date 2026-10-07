# Tasks: Role-Based Access & View Separation

Derived from **spec.md** ACs. Each task maps to one or more ACs and contains task-local Test Requirements (`rule` or `rubric`).

## Task 1: Login page → admin post-login redirect & admin hint

**Map:** AC-1.1, AC-1.2, AC-1.3, AC-NFR-2

### Scope
- [AuthenticationEndpoints.cs](file:///c:/Users/Tabitha/VerdeEnergy/WebUI/Services/AuthenticationEndpoints.cs#L92-L175): After `/auth/login` succeeds, check the `returnUrl` — if it is missing, empty, `/login`, `/register`, or `/`, AND the signed-in user is in `Admin` role → redirect to `/operations` instead of `/`. For customers keep existing behavior (redirect to `/account` or passed returnUrl).
- [Login.razor](file:///c:/Users/Tabitha/VerdeEnergy/WebUI/Components/Pages/Login.razor#L1-L23): Add an "Admin sign-in" hint box under the heading (muted text, never shows a password) that says: "Verde Energy administrator? Use the email configured on the Operations server to sign in to the internal desk. For support, check with the Verde Energy admin team."

### Test Requirements

| ID | Type | Pass condition | Evidence |
|---|---|---|---|
| TR 1.1 | rule | Build succeeds (0 errors) | `dotnet build` output |
| TR 1.2 | rule | Sign in with admin account and no returnUrl → redirected to `/operations` | Browser GET `/operations` 200 after login |
| TR 1.3 | rule | Sign in with a customer-only account and no returnUrl → redirected to `/book-drive` (keeps customer behavior) | Browser GET `/book-drive` 200 after login |
| TR 1.4 | rule | Login page HTML source contains the word "administrator" (hint present) | Browser snapshot / view-source |

---

## Task 2: Registration → force Customer role, redirect to /book-drive

**Map:** AC-2.1, AC-2.2

### Scope
- [AuthenticationEndpoints.cs](file:///c:/Users/Tabitha/VerdeEnergy/WebUI/Services/AuthenticationEndpoints.cs#L17-L91): Confirm `/auth/register` always assigns the new user to `Customer` role (never `Admin`). If a `returnUrl` was passed it is used; otherwise redirect to `/book-drive` (not `/account`). Make sure `returnUrl` is validated as local (no open redirect) via standard `IsLocalUrl`.

### Test Requirements

| ID | Type | Pass condition | Evidence |
|---|---|---|---|
| TR 2.1 | rule | New registered user, after login, has exactly the `Customer` role claim (no `Admin`) | EF query on AspNetUserRoles via Users.razor or test |
| TR 2.2 | rule | Post-register with no returnUrl → browser lands at `/book-drive` | Browser URL after registration |

---

## Task 3: Navigation separation (NavMenu, PublicLayout, Home)

**Map:** AC-3.1, AC-3.2, AC-3.3, AC-NFR-3

### Scope
- [NavMenu.razor](file:///c:/Users/Tabitha/VerdeEnergy/WebUI/Components/Layout/NavMenu.razor#L1-L40): Confirm the sidebar already wraps admin links in `<AuthorizeView Roles="Admin">`. Add a small improvement: wrap the "Home" NavLink in an outer `AuthenticateView` (any role) so unauthenticated sidebar does not show links. Make sure sample pages Counter / Weather do NOT appear in sidebar (they are already missing, leave them off — but we'll add admin-only attributes to those pages in Task 4).
- [PublicLayout.razor](file:///c:/Users/Tabitha/VerdeEnergy/WebUI/Components/Layout/PublicLayout.razor#L1-L70): Confirm the Authorized block contains `<AuthorizeView Roles="Admin">` with an Operations link.
- [Home.razor](file:///c:/Users/Tabitha/VerdeEnergy/WebUI/Components/Pages/Home.razor#L47-L59): Verify hero nav has the nested AuthorizeView role check for "Operations" CTA.

### Test Requirements

| ID | Type | Pass condition | Evidence |
|---|---|---|---|
| TR 3.1 | rule | As a Customer-only user, NavMenu sidebar has no "Operations" / "Users" / "Fleet" / "Installations" / "Finance" / "Consulting" text | Browser snapshot of sidebar |
| TR 3.2 | rule | As Admin user, PublicLayout header shows "Operations Desk" link | Browser snapshot of top bar |
| TR 3.3 | rule | As Admin user, Home.razor hero has a visible "Operations" CTA button | Browser snapshot of home hero |

---

## Task 4: Page authorization attribute audit + add missing

**Map:** AC-4.1

### Scope
For each page, confirm the `[Authorize(...)]` attribute matches the Authorization matrix in spec FR-4, and add missing ones:
- `/counter` ([Counter.razor](file:///c:/Users/Tabitha/VerdeEnergy/WebUI/Components/Pages/Counter.razor)) → add `@attribute [Authorize(Roles = "Admin")]`.
- `/weather` ([Weather.razor](file:///c:/Users/Tabitha/VerdeEnergy/WebUI/Components/Pages/Weather.razor)) → add `@attribute [Authorize(Roles = "Admin")]`.
- Verify existing: Operations, Users, Vehicles, Quotes, Finance, Consulting all have `[Authorize(Roles = "Admin")]`.
- Verify Book-drive, Track, Account, Logout pages: confirm `[Authorize]` (any role) is present.
- Verify Login/Register/Request/Home: confirm `[AllowAnonymous]` or no auth (for Home).
- Confirm Error/NotFound: no auth attribute (middleware fallback pages, must always render).

### Test Requirements

| ID | Type | Pass condition | Evidence |
|---|---|---|---|
| TR 4.1 | rule | Grep for `@page "/counter"` finds the page with `[Authorize(Roles = "Admin")]` | Grep output |
| TR 4.2 | rule | Direct GET `/counter` by Customer-only role user → 302 redirect to `/login` | Browser network log / server 302 |
| TR 4.3 | rule | Direct GET `/operations` by Customer-only role user → 302 redirect to `/login` | Browser network log / server 302 |

---

## Task 5: DriveBooking ownership & Admin "book on behalf of"

**Map:** AC-5.1

### Scope
- [DriveBooking.razor](file:///c:/Users/Tabitha/VerdeEnergy/WebUI/Components/Pages/DriveBooking.razor#L268-L358): In `SubmitBooking`:
  - If `User.IsInRole("Admin")`: add a customer selector dropdown at the top of the form (list of users; default = admin self). Capture `selectedCustomerUserId`.
  - Before saving the `TripRequest`, if user is NOT admin, overwrite `CustomerUserId` to the signed-in user's ID (ignore any tampered value). If user IS admin, use `selectedCustomerUserId`.
  - Same rule for `Transaction.OwnerUserId`.

### Test Requirements

| ID | Type | Pass condition | Evidence |
|---|---|---|---|
| TR 5.1 | rule | Customer user books → `TripRequest.CustomerUserId` matches `NameIdentifier` claim of signed-in user. | EF SQL query / Operations table row |
| TR 5.2 | rule | Admin user has a dropdown of all customer users available in the DriveBooking form. | Browser snapshot of DriveBooking |
| TR 5.3 | rule | Admin user booking on behalf of a selected customer saves `CustomerUserId` = the selected user's ID (not the admin's ID). | Operations table row |

---

## Task 6: Track.razor strict scoping (Customer vs Admin lookup)

**Map:** AC-5.2, AC-5.3, AC-NFR-4

### Scope
- [Track.razor](file:///c:/Users/Tabitha/VerdeEnergy/WebUI/Components/Pages/Track.razor#L75-L100) `FindTrip` method:
  - If signed-in user is in `Admin` role → filter only by `trip.Id == tripId` (no ownership filter).
  - If user is only in `Customer` role → filter by `trip.Id == tripId && trip.CustomerUserId == userId`.
  - Add a check: if trip lookup matches but the signed-in user is Admin, show an extra row "Owned by: {customerFullName} {customerEmail}" (usability for admin).

### Test Requirements

| ID | Type | Pass condition | Evidence |
|---|---|---|---|
| TR 6.1 | rule | Customer user searching another customer's valid TRP returns "We couldn't find a request for your account with that reference." | Browser text |
| TR 6.2 | rule | Admin user searching any valid TRP (whether theirs or not) returns the trip details. | Browser text |

---

## Task 7: Account.razor — profile fields, admin toggle "Show all customers", Admin shortcuts tiles

**Map:** AC-4.2, AC-6.1, AC-6.2, AC-NFR-4

### Scope
- [Account.razor](file:///c:/Users/Tabitha/VerdeEnergy/WebUI/Components/Pages/Account.razor#L1-L85):
  - **Profile read-only panel**: Show `FullName` (identity), `Email` (identity), `PhoneNumber` (identity, "Not provided" if null/empty).
  - **Admin-only switch** (if `isAdmin`): checkbox "Show all customers (admin view)". When checked:
    - Trips query returns all `TripRequests` (no userId filter), add an "Owner" column showing `trip.CustomerName` or fallback to `user.FullName` from `ApplicationUser` linked by `CustomerUserId`.
    - Transactions query returns all `Transactions`, add an "Owner" column showing linked owner name.
  - **Admin shortcuts tile grid** (if `isAdmin`): four inline tiles linking to `/operations`, `/users`, `/vehicles`, `/finance`.
  - Preserve the existing customer-only layout for non-admin users (exact same look-and-feel).

### Test Requirements

| ID | Type | Pass condition | Evidence |
|---|---|---|---|
| TR 7.1 | rule | As Customer, `/account` shows only that customer's own trips and payments (no owner column visible). | Browser table snapshot |
| TR 7.2 | rule | As Admin, `/account` default state (checkbox unchecked) shows only the admin's own trips. When checked, trips and transactions tables include an "Owner" column with the name of the owner user. | Browser table snapshots (two states) |
| TR 7.3 | rule | As Admin, `/account` has four visible tile links: Operations, Users, Fleet (Vehicles), Finance. | Browser snapshot of tiles |
| TR 7.4 | rule | `/account` renders a read-only "Your profile" section with Name, Email, Phone. | Browser snapshot of panel |

---

## Task 8: Verify build, runtime smoke test, and end-to-end role flows

**Map:** AC-NFR-1, AC-NFR-2, AC-NFR-3, AC-NFR-4

### Scope
- `dotnet build` in WebUI directory → 0 errors.
- Smoke-test both flows using the browser:
  1. **Admin flow**: Open `/login` → sign in with admin credentials → confirm redirect to `/operations` → click sidebar to Users, Fleet, Installations, Finance, Consulting (all 200, load fully).
  2. **Customer flow**: Open `/register` → create a new customer → confirm redirect to `/book-drive` → try to GET `/operations` by URL → confirm redirect to `/login` → go to `/track` → try lookup of a trip you don't own → confirm "not found for your account".

### Test Requirements

| ID | Type | Pass condition | Evidence |
|---|---|---|---|
| TR 8.1 | rule | `dotnet build` returns 0 errors. | Build log |
| TR 8.2 | rule | Admin smoke-test flow passes (all 5 admin pages load). | Screenshots + no exceptions in server log |
| TR 8.3 | rule | Customer smoke-test flow passes (/operations is denied, /track denies other-owner refs). | Screenshots + redirects |
