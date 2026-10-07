# Spec: Role-Based Access & View Separation (Admin vs Customer)

## Problem
VerdeEnergy needs a clear role-separated user experience:
- **Administrator** (`Admin` role) should sign in with a single known admin credential and see **everything** — every admin page, every user's trip, every payment, internal forms (Operations, Users, Fleet, Installations, Finance, Consulting), plus their own customer-level activity.
- **Customer** (`Customer` role only) should be able to self-register, sign in, and see **only their own data** — their own trips, their own payments, their own booking and tracking — and never see admin pages, admin navigation, nor any other user's data.

Today the pages exist and many already have `[Authorize(Roles = "Admin")]` attributes, but the user experience needs to be strengthened so:
1. Admin login works reliably and lands Admin users on the Operations desk (the "home" for internal staff).
2. Navigations are fully role-scoped — a Customer never sees admin links.
3. Customer data access on shared (no `Roles=` attribute) pages is enforced server-side by user-ID filtering (no leakage).
4. `/account` for Admins additionally shows the admin shortcuts so Admin can still use the "customer" account view but quickly jump to internal admin pages.
5. A clear "Forgot / I need a password reset" fall-back message appears on the Login page for Customers, so admin can assist.

## Users & Goals

| Actor | Goal |
|---|---|
| System Admin (Tabitha) | Sign in with `utujetabitha@gmail.com / Admin123!` and immediately reach the internal Operations desk; navigate to and operate every internal form (all trips, all payments, users, fleet, installations, finance, consulting). |
| Verde Customer (self-registered) | Create account via `/register`, sign in via `/login`, book charging, track requests, see only the customer's own trips and payments on the "My account" page, never see admin links. |

## Functional Requirements

### FR-1: Admin authentication and landing
- The initial Admin seed user exists as defined in `appsettings.json` (email `utujetabitha@gmail.com`, password `Admin123!`) and is assigned both `Admin` and `Customer` roles.
- After a successful admin sign-in (user has `Admin` role):
  - If the `returnUrl` query parameter points to `/login`, `/register`, or `/`, the server redirects the Admin user to `/operations`.
  - Otherwise the server respects the provided `returnUrl`.
- Login page must show a subtle "Admin sign-in" note telling admins which email / credentials to use (hint text only, never disclose a password).

### FR-2: Customer self-registration flow
- `/register` always assigns the new user to the `Customer` role (never `Admin`).
- After a successful Customer registration and sign-in, redirect to `/book-drive` (or the provided `returnUrl`).
- A new Customer's first sign-in shows no admin pages in any navigation.

### FR-3: Navigation separation
- **MainLayout sidebar (NavMenu)**
  - Admin role links (Operations, Users, Fleet, Installations, Finance, Consulting) are rendered **only** when the user is in the `Admin` role.
  - "Home" renders for every authenticated user.
  - Customer-scoped links (Book a service, Track request, My account, Sign out) are rendered for any authenticated user.
  - "Sign in" renders when not authenticated.
- **PublicLayout top bar (Public pages: Home / Login / Register / Book-drive / Track / Account)**
  - `Admin` users see "Operations Desk" link.
  - Any authenticated user sees "Book a service", "Track request", "My account", "Sign out".
  - Anonymous users see only "Sign in", "Create account".
- **Public home page hero nav (Home.razor)**
  - Admin authenticated users see an "Operations" call-to-action.
  - Other authenticated users see "My account" call-to-action.
  - Anonymous users see "Sign in" call-to-action.

### FR-4: Page authorization matrix
Every page must be reachable by the correct role and blocked to everyone else (redirected to Login, not to blank 404 / 500):

| Page | Route | Authorized role(s) | Notes |
|---|---|---|---|
| Home | `/` | Everyone (anonymous + authenticated) | Marketing / landing |
| Request (public) | `/request` | Everyone (anonymous + authenticated) | Estimate form for all |
| Login | `/login` | Anonymous + authenticated fallback | `AllowAnonymous` |
| Register | `/register` | Anonymous + authenticated fallback | `AllowAnonymous` |
| Logout | `/logout` | Authenticated any role | Ends session |
| Drive booking | `/book-drive` | Authenticated (Admin or Customer) | Auto-populate customer user ID |
| Track | `/track` | Authenticated (Admin or Customer) | Customer sees own trips only; Admin can additionally see any trip by reference without ownership filter |
| My account | `/account` | Authenticated (Admin or Customer) | Customer scopes to own trips/payments; Admin additionally sees admin shortcut CTAs and can optionally view all if admin switch enabled (see AC-4.2) |
| Operations | `/operations` | `Admin` only | All trips, all sessions, all assignments |
| Users | `/users` | `Admin` only | User management |
| Vehicles / Fleet | `/vehicles` | `Admin` only | Fleet registry |
| Installations / Quotes | `/quotes` | `Admin` only | Internal installations quotes & forms |
| Finance | `/finance` | `Admin` only | All transactions, metrics, exports |
| Consulting | `/consulting` | `Admin` only | Corporate engagements board |
| Counter, Weather sample pages | `/counter`, `/weather` | `Admin` only (already protected by `MainLayout` sidebar visibility; for safety add `[Authorize(Roles = "Admin")]`) | Unused sample pages should not be reachable by Customers |
| Error | `/Error` | Everyone (middleware exception path) | `AllowAnonymous`-equivalent |
| 404 / NotFound | catch-all | Everyone (middleware) | `AllowAnonymous`-equivalent |

### FR-5: Data scope enforcement (critical — no cross-user leakage)
- **DriveBooking.razor** creates trips and transactions: the `CustomerUserId` on the new `TripRequest` and the `OwnerUserId` on the new `Transaction` **must always be set from the authenticated user's `NameIdentifier` claim**. For Admin users booking on behalf of a customer, allow the form to choose a customer (via dropdown of users in Customer role) **only if the currently authenticated user is Admin**. Otherwise the form ignores any submitted customer identifier and re-assigns it to the signed-in user.
- **Account.razor ("My account")**
  - `Customer` role only (i.e., not `Admin`): shows trips filtered by `trip.CustomerUserId == currentUserId`.
  - `Admin` role: by default also shows only the admin user's own trips (so the admin can still use the customer view), but adds an "Admin mode: show all customers" toggle (checkbox / switch). When the switch is on, Account.razor shows **all** trips and **all** payments plus an "Owned by" column linking each row to the trip owner.
  - Transactions table: same scoping rule as trips.
- **Track.razor**
  - If signed in user is a `Customer`, the trip lookup **also** requires `trip.CustomerUserId == currentUserId`. Customers cannot search each other's trip references even if they know one.
  - If signed in user is `Admin`, the trip lookup allows any trip reference (no `CustomerUserId` filter), so Admin can look up any customer's trip by TRP number.
  - If not signed in at all (should not happen because page is `[Authorize]`), Track must return an error prompt "Sign in to view a request."

### FR-6: Account page profile editing (usability)
- `Account.razor` shows the user's full name, email, and phone number (read-only) pulled from the Identity `ApplicationUser` entity (not legacy fields on trips).
- Admin users see an additional "Administrative view" section on Account.razor with four shortcut tiles: Operations, Users, Fleet, Finance. (PublicLayout CTA section already has a partial "Operations desk →" link; extend it into a small tile grid for admins.)

## Non-Functional Requirements

### NFR-1: Build and run with 0 errors
- `dotnet build` in `WebUI/` returns 0 errors.
- Runtime: navigating to each page in the authorization matrix returns either the page (if authorized) or a clean redirect to `/login?ReturnUrl=...` (never a crash, never a blank red banner).

### NFR-2: No secrets or password disclosed in UI or markup
- Admin hint on login page only suggests the email (`utujetabitha@gmail.com`). Password never appears in markup.
- Customer registration success messages never include a submitted password in the redirect query string.

### NFR-3: Compatible with existing Identity bootstrap
- `InitializeIdentityRolesAsync` in `AuthenticationEndpoints.cs` continues to work. It seeds Admin + Customer roles, creates the seed admin user, and assigns both roles to that user. No schema / migration changes are required if existing data is preserved.

### NFR-4: No new packages
- Implement using only dependencies already in `WebUI.csproj` (ASP.NET Core Identity, Blazor Server, EF Core).

## Constraints, Dependencies, Assumptions
- **Constraints**: Must run without a new database migration. Existing Identity tables + `TripRequests.CustomerUserId`, `Transactions.OwnerUserId` columns are already in place (migration `AddTripBookingAndPaymentData` / `AddIdentityAndTripOwnership`).
- **Dependencies**: Current app reads admin seed credentials from `Configuration["InitialAdmin:Email"]` and `Configuration["InitialAdmin:Password"]`. We reuse those values.
- **Assumptions**: SQL Server connection is functional (localDB or `Server=UTUJE` as configured). If no customers exist yet in the DB, the Admin's "on behalf of" dropdown in DriveBooking simply shows only the admin user as a target (acceptable).

## Acceptance Criteria

All ACs are typed. `rule` = binary pass/fail; `rubric` = scored quality dimension.

| ID | Type | Text |
|---|---|---|
| AC-1.1 | rule | Sign in with `utujetabitha@gmail.com / Admin123!` from `/login` (no ReturnUrl) results in an HTTP 302 redirect to `/operations`. |
| AC-1.2 | rule | After admin sign-in, the Operations page loads with zero exceptions. |
| AC-1.3 | rule | Login page includes visible, non-obtrusive admin hint text that only mentions the admin email address. |
| AC-2.1 | rule | Submitting `/auth/register` with valid Customer fields creates a user who is in the `Customer` role only. |
| AC-2.2 | rule | After new Customer registration → sign-in completes, browser lands on `/book-drive` (or the provided `returnUrl`, if any). |
| AC-3.1 | rule | In NavMenu (sidebar), a Customer user does NOT see the following links: Operations, Users, Fleet, Installations, Finance, Consulting. |
| AC-3.2 | rule | In PublicLayout top bar, an Admin user sees the "Operations Desk" link. A Customer user does NOT see it. |
| AC-3.3 | rule | In Home.razor hero nav, after an Admin signs in, there is a visible "Operations" CTA button. |
| AC-4.1 | rule | Direct URL access `/operations`, `/users`, `/vehicles`, `/quotes`, `/finance`, `/consulting` (entered by a signed-in Customer user who is role Customer only) results in redirect to `/login` (not crash, not blank page, not 200 OK with the page content). |
| AC-4.2 | rule | `/account` for an Admin user shows: (a) a toggle to "Show all customers" (unchecked by default → admin sees only own trips); (b) checked → trips and payments tables contain an "Owner" column showing trip owner name/email. |
| AC-5.1 | rule | When a signed-in Customer creates a booking on `/book-drive`, the saved `TripRequest.CustomerUserId` equals the `NameIdentifier` claim of the signed-in user. |
| AC-5.2 | rule | When a signed-in Customer searches `/track` with a reference belonging to a **different** user, the result is "We couldn't find a request for your account with that reference." (no trip details shown). |
| AC-5.3 | rule | When Admin searches `/track` with any valid reference, the trip is shown regardless of owner. |
| AC-6.1 | rule | `/account` shows the signed-in user's full name, email, and phone from `ApplicationUser` (or "Not provided" if phone is blank). |
| AC-6.2 | rule | Admin `/account` renders shortcut tiles for Operations, Users, Fleet, Finance. |
| AC-NFR-1 | rule | Running `dotnet build` in the `WebUI` folder reports **0 errors**. |
| AC-NFR-2 | rubric | Login flow usability: scale 0-2. 2 = clean, clear role-specific prompts; 1 = functional but hints missing; 0 = confusing or leaks a password. |
| AC-NFR-3 | rubric | Role separation clarity: scale 0-2. 2 = no admin links or hints leak to Customer; 1 = one minor link visible but page itself blocks; 0 = Customer reaches any admin page. |
| AC-NFR-4 | rubric | Scope correctness on /account and /track: scale 0-2. 2 = strict per-user filter applied; 1 = leaks one field type; 0 = full cross-view possible. Pass threshold ≥ 2. |
