# Software Requirements Specification

## 1. Introduction

### 1.1 Purpose

This document defines the software requirements for BookYourStay, a booking platform for hospitality and experience-based products. It is intended to guide product planning, backend and frontend implementation, data design, and future testing.

### 1.2 Scope

BookYourStay is a multi-role platform that allows:

- customers to discover and book rooms, packages, activities, and event venues
- partners to onboard, manage listings, inventory, pricing, and bookings
- admins to approve partners and listings, manage reference data, and moderate platform activity

The platform will support online payment flows, booking lifecycle tracking, optional add-on services, and review collection for completed bookings.

### 1.3 Business Goals

- unify multiple hospitality booking types into a single platform
- allow partners to self-manage supply while preserving platform controls
- provide a reliable booking and payment flow with auditability
- start as a modular monolith that can scale feature by feature

### 1.4 Definitions

- Customer: end user who browses and creates bookings
- Partner: business user who manages properties, offerings, and bookings
- Admin: internal operator with approval and moderation powers
- Property: top-level listing such as hotel, resort, or venue business
- Offering: bookable item such as room, package, activity, or event venue
- Add-on: optional extra attached to a booking, such as transport or catering
- Quote: pre-booking price calculation returned before booking creation

### 1.5 References

- Repository docs: [README.md](README.md)
- Backend docs: [backend.md](backend.md)
- Frontend docs: [frontend.md](frontend.md)
- Database draft: [../schema.sql](../schema.sql)

### 1.6 Context Source

This SRS is aligned against the current `schema.sql` draft. Where product terms are broad, the schema provides the current implementation-oriented context for domains, states, and data relationships.

## 2. Overall Description

### 2.1 Product Perspective

BookYourStay is a platform product composed of:

- a customer-facing web application
- an ASP.NET Core backend API
- a PostgreSQL database
- object storage for media assets
- a payment integration in test mode initially

The repository is organized as a monorepo with separate app workspaces for backend and frontend.

At the data-model level, the system is split into these explicit schemas:

- `iam` for users, identities, sessions, roles, addresses, and token workflows
- `core` for partner organizations and partner-user relationships
- `catalog` for listing content and sellable offerings
- `inventory` for date-based inventory and time-slot availability
- `booking` for bookings, coupons, guest records, and typed booking items
- `billing` for payments, refunds, and payment state history
- `integration` for webhook events and processing state
- `engagement` for reviews and moderation state

### 2.2 Product Functions

At a high level, the system will:

- authenticate users
- support customer, partner, and admin roles
- allow partner onboarding and approval
- allow property and offering management
- support search, filtering, and detail pages
- calculate quotes and create bookings
- support coupons, payments, and booking status tracking
- support reviews and moderation

### 2.3 User Classes

#### Customer

Customers are expected to:

- browse public listings
- compare offerings by date, price, and amenities
- create and manage their bookings
- pay online
- review completed bookings

#### Partner

Partners are expected to:

- create and maintain listing content
- manage pricing and availability
- respond to and track bookings
- maintain add-ons and optional services

#### Admin

Admins are expected to:

- approve or reject partner applications
- manage reference data and moderation workflows
- monitor booking and payment operations

### 2.4 Operating Environment

- Frontend: modern desktop and mobile web browsers
- Backend: ASP.NET Core / .NET 10 runtime
- Database: PostgreSQL 15+
- Local infrastructure: Docker Compose
- Frontend stack: Next.js, React, TypeScript

### 2.5 Constraints

- backend and frontend should remain in one monorepo
- initial payment support may run in test mode only
- raw card data must not be stored by the platform
- the system should start as a modular monolith, not microservices

### 2.6 Assumptions and Dependencies

- partners will manage the accuracy of inventory and pricing inputs
- availability calculations depend on partner-maintained inventory data
- payment confirmation depends on backend-validated payment state
- object storage will be used for listing media and other uploaded assets
- some location fields may be stored as API-driven codes plus display names rather than fully normalized geography tables

## 3. Scope by Release

### 3.1 MVP In Scope

- account registration and login
- password reset
- partner onboarding and approval
- property listing and management
- room, package, activity, and venue setup
- search and filtering
- quote generation
- booking creation
- coupon support
- payment flow in test mode
- booking status tracking
- reviews and moderation

### 3.2 Out of Scope for MVP

- loyalty points
- referral rewards
- corporate booking workflows
- advanced analytics
- dynamic pricing engine
- real third-party transport integrations

## 4. Use Cases

### 4.1 UC-001 Customer Registration and Sign-In

- Primary actor: Customer
- Preconditions: Customer is not yet authenticated.
- Trigger: Customer chooses to register or log in.
- Main flow:
  1. Customer submits registration or login credentials.
  2. System validates the request.
  3. System creates or verifies the user identity.
  4. System creates authenticated session state.
  5. System returns authenticated user context to the client.
- Postconditions:
  - User is registered or authenticated.
  - Session state exists for the authenticated user.

### 4.2 UC-002 Customer Search and Booking

- Primary actor: Customer
- Preconditions:
  - Relevant property and offering data exists.
  - Inventory and pricing are available for the selected dates or slots.
- Trigger: Customer searches and chooses to book an offering.
- Main flow:
  1. Customer searches by location, date, and optional filters.
  2. System returns matching public listings and offerings.
  3. Customer opens a listing and selects an offering.
  4. System calculates a quote using pricing, discounts, and optional add-ons.
  5. Customer confirms booking details.
  6. System validates availability and creates a booking.
  7. System places the booking into an unpaid lifecycle state.
- Alternate or exception flows:
  - Inventory is no longer available.
  - Coupon is invalid or expired.
  - Quote changes due to updated pricing or availability.
- Postconditions:
  - A booking record exists.
  - Booking line items and guest context are recorded.
  - Inventory remains ready to be reserved upon confirmation.

### 4.3 UC-003 Customer Payment Completion

- Primary actor: Customer
- Supporting actor: Payment provider
- Preconditions:
  - A booking exists in a payment-pending state.
  - A payment record or payment intent can be created.
- Trigger: Customer proceeds to payment.
- Main flow:
  1. Backend creates a payment session or payment intent.
  2. Customer completes the provider-facing payment step.
  3. Provider sends trusted payment state back through backend-controlled validation or webhook flow.
  4. System updates payment status.
  5. System updates booking status when payment conditions are satisfied.
- Alternate or exception flows:
  - Payment requires additional action.
  - Payment fails.
  - Payment is cancelled or expires.
- Postconditions:
  - Payment state is persisted.
  - Booking is either confirmed or remains in a non-confirmed payment lifecycle state.

### 4.4 UC-004 Partner Onboarding and Approval

- Primary actor: Partner applicant
- Supporting actor: Admin
- Preconditions: Applicant does not yet have an approved partner organization.
- Trigger: Applicant submits partner onboarding details.
- Main flow:
  1. Applicant submits business profile and onboarding data.
  2. System creates a partner organization in a pending state.
  3. Admin reviews submitted information.
  4. Admin approves or rejects the partner.
  5. System updates partner status accordingly.
- Postconditions:
  - Partner organization is stored with an approval state.
  - Approved partners may proceed to listing management.

### 4.5 UC-005 Partner Listing and Inventory Management

- Primary actor: Approved partner user
- Preconditions:
  - Partner organization is approved.
  - Partner user has sufficient access level.
- Trigger: Partner creates or edits operational content.
- Main flow:
  1. Partner creates or updates a property.
  2. Partner adds offerings such as rooms, packages, activities, venues, transport, or add-ons.
  3. Partner maintains pricing, capacity, and availability data.
  4. System validates and stores those updates.
  5. Public discovery and quote behavior reflect published, active, and available data.
- Postconditions:
  - Listing and inventory data are persisted.
  - Discovery and booking flows can use the updated data.

### 4.6 UC-006 Admin Moderation and Operations

- Primary actor: Admin
- Preconditions: Admin is authenticated with appropriate privileges.
- Trigger: Admin reviews platform operations or content.
- Main flow:
  1. Admin reviews partner, property, booking, payment, or review data.
  2. Admin performs approval, publication, moderation, or support actions.
  3. System updates the affected operational records.
- Postconditions:
  - Moderation or operational decision is persisted.
  - Relevant lifecycle or visibility state is updated.

## 5. External Interface Requirements

### 5.1 User Interface Requirements

- the frontend must support responsive layouts for desktop and mobile
- public discovery pages must allow search, filter, and detail navigation
- authenticated users must have clear role-appropriate navigation
- booking flows must clearly display price breakdowns, dates, and add-ons
- partner dashboards must support CRUD flows for listings and inventory
- admin screens must support approval and moderation workflows

### 5.2 API Requirements

- the backend must expose versioned HTTP APIs under `/api/v1`
- protected endpoints must require authenticated access
- APIs should return consistent success and error response shapes
- API validation errors should identify fields where possible

### 5.3 Software Interfaces

- PostgreSQL for transactional data
- S3-compatible object storage for uploaded assets
- payment provider integration for payment intent and webhook flows

### 5.4 Communications Requirements

- all production traffic should use HTTPS
- webhook handlers must validate provider events
- API consumers must use bearer token authentication for protected APIs

## 6. Functional Requirements

### 6.1 Authentication and Identity

- FR-AUTH-001: users must be able to register with email and password
- FR-AUTH-002: users must be able to log in with valid credentials
- FR-AUTH-003: users must be able to log out
- FR-AUTH-004: users must be able to request a password reset
- FR-AUTH-005: users must be able to reset passwords with a valid reset token
- FR-AUTH-006: the system must support role-aware authorization decisions
- FR-AUTH-007: the system must maintain session-related state for authenticated access
- FR-AUTH-008: the system must support multiple identity providers, including local, Google, and phone-based OTP models
- FR-AUTH-009: the system must support email verification token workflows
- FR-AUTH-010: token-bearing flows must store hashed token values rather than raw secrets where possible

### 6.2 User and Role Management

- FR-USER-001: the system must distinguish customer, partner, and admin capabilities
- FR-USER-002: customer profile information must be retrievable and editable
- FR-USER-003: partner users must be associated with a partner organization
- FR-USER-004: the system must support multiple roles per user where needed
- FR-USER-005: user records must support email and phone verification timestamps
- FR-USER-006: the system should support user address records for operational and profile use cases

### 6.3 Partner Onboarding

- FR-PARTNER-001: partner applicants must be able to submit onboarding information
- FR-PARTNER-002: admins must be able to review partner applications
- FR-PARTNER-003: admins must be able to approve or reject partner applications
- FR-PARTNER-004: only approved partners may publish operational listings
- FR-PARTNER-005: partner organizations must support multiple partner users with access levels such as owner, manager, finance, operations, and viewer

### 6.4 Property Management

- FR-PROP-001: partners must be able to create a property profile
- FR-PROP-002: partners must be able to update property details
- FR-PROP-003: partners must be able to upload and manage property media
- FR-PROP-004: partners must be able to define amenities and basic policies
- FR-PROP-005: admins must be able to publish or unpublish properties
- FR-PROP-006: properties must support structured location, contact, star rating, and check-in/check-out metadata
- FR-PROP-007: properties must support lifecycle states such as draft, pending review, published, suspended, and archived
- FR-PROP-008: properties must support publication metadata such as featured flags and published timestamps

### 6.5 Offering Management

- FR-OFFER-001: partners must be able to create room types
- FR-OFFER-002: partners must be able to create staycation and daycation packages
- FR-OFFER-003: partners must be able to create activities
- FR-OFFER-004: partners must be able to create event venues
- FR-OFFER-005: partners must be able to create transport options and service add-ons
- FR-OFFER-006: partners must be able to update and archive offerings
- FR-OFFER-007: room types must support occupancy, bed, size, and base pricing definitions
- FR-OFFER-008: packages must support package-type classification, validity windows, and optional room-type linkage
- FR-OFFER-009: activities must support duration and restrictions metadata
- FR-OFFER-010: event venues must support placement classification and capacity rules
- FR-OFFER-011: transport options must support transport type, trip type, route labeling, and manual confirmation flags
- FR-OFFER-012: service add-ons must support category and pricing-model classification

### 6.6 Inventory and Pricing

- FR-INV-001: partners must be able to manage inventory and availability
- FR-INV-002: partners must be able to define prices and pricing rules for offerings
- FR-INV-003: partners must be able to configure blackout dates or unavailable periods
- FR-INV-004: search and quote results must reflect current inventory and pricing data
- FR-INV-005: room inventory must support reserved inventory, out-of-service inventory, stay restrictions, and arrival/departure closure flags
- FR-INV-006: package inventory must support per-date inventory and price overrides
- FR-INV-007: activity inventory must support time slots, capacity, reserved counts, and slot statuses
- FR-INV-008: venue inventory must support time slots, capacity, price overrides, and blockable statuses

### 6.7 Discovery and Search

- FR-DISC-001: users must be able to search by location and date
- FR-DISC-002: users must be able to filter by price, amenities, rating, capacity, and availability
- FR-DISC-003: users must be able to browse public property and offering details
- FR-DISC-004: search results should support sorting by price, rating, and relevance or popularity
- FR-DISC-005: public discovery should only expose listings and offerings that are currently intended to be visible

### 6.8 Quote and Booking

- FR-BOOK-001: the system must provide a quote before booking creation
- FR-BOOK-002: the quote must include base price, add-ons, discounts, and applicable fees
- FR-BOOK-003: users must be able to create bookings for supported offering types
- FR-BOOK-004: the system must validate inventory before accepting a booking
- FR-BOOK-005: newly created unpaid bookings must enter a pending payment state
- FR-BOOK-006: confirmed bookings must reserve corresponding inventory
- FR-BOOK-007: users must be able to view booking status and booking details
- FR-BOOK-008: users must be able to request cancellation for eligible bookings
- FR-BOOK-009: bookings must record customer, property, and partner ownership context
- FR-BOOK-010: bookings must support both date-based stay ranges and time-based activity or venue windows
- FR-BOOK-011: bookings must support primary and additional guest records
- FR-BOOK-012: bookings must support typed line items for rooms, packages, activities, venues, transport, and add-ons
- FR-BOOK-013: bookings must support source-channel and special-request metadata
- FR-BOOK-014: bookings must support lifecycle timestamps such as cancelled and completed dates
- FR-BOOK-015: booking statuses must support at least draft, pending payment, confirmed, partially paid, cancelled, completed, refund pending, refunded, and expired
- FR-BOOK-016: booking state changes must be historized

### 6.9 Coupons and Discounts

- FR-COUPON-001: the system must support coupon validation
- FR-COUPON-002: admins must be able to create and manage coupon rules
- FR-COUPON-003: valid coupons must affect quote and booking totals
- FR-COUPON-004: coupons must support fixed and percentage discount models
- FR-COUPON-005: coupons must support validity windows, minimum booking amounts, and usage limits
- FR-COUPON-006: successful usage must create a coupon redemption record tied to the booking and user

### 6.10 Payments

- FR-PAY-001: the backend must create payment intents or equivalent payment sessions
- FR-PAY-002: payment confirmation must not rely only on frontend success responses
- FR-PAY-003: backend payment state must be updated only after trusted validation
- FR-PAY-004: payment and refund records must retain state history or equivalent audit data
- FR-PAY-005: the system must support refund-related workflows and statuses
- FR-PAY-006: payment records must support multiple providers and method types
- FR-PAY-007: payment records must support provider-specific identifiers such as payment intent, charge, and refund references
- FR-PAY-008: payment creation should support idempotency keys
- FR-PAY-009: payment statuses must support pending, requires action, authorized, paid, failed, cancelled, refund pending, refunded, and partially refunded
- FR-PAY-010: refund statuses must support pending, succeeded, failed, and cancelled
- FR-PAY-011: payment state changes must be historized

### 6.11 Reviews

- FR-REV-001: only completed bookings should be eligible for review
- FR-REV-002: reviews must support rating and comment fields
- FR-REV-003: users must be able to edit or remove their own reviews within allowed rules
- FR-REV-004: admins must be able to moderate or hide inappropriate reviews
- FR-REV-005: reviews must be linked to the booking, customer, and property involved
- FR-REV-006: review moderation states must support published, hidden, and flagged
- FR-REV-007: partner responses to reviews should be supported

### 6.12 Administration

- FR-ADMIN-001: admins must be able to monitor partner, property, booking, payment, and review activity
- FR-ADMIN-002: admins must be able to manage reference or taxonomy data where required
- FR-ADMIN-003: admins must be able to moderate platform content and operations

### 6.13 Webhooks and External Events

- FR-INT-001: the system must store inbound webhook events for audit and retry purposes
- FR-INT-002: webhook events must support processing states such as pending, processed, failed, and ignored
- FR-INT-003: webhook events should be deduplicated by provider and provider event identifier
- FR-INT-004: webhook events may reference the payment record they affected

## 7. Data Requirements

### 7.1 Core Entity Groups

The system data model is expected to include:

- users, identities, sessions, and roles
- email verification and password reset tokens
- user addresses
- partners and partner users
- properties, media, policies, and amenities
- offerings such as rooms, packages, activities, and venues
- transport options and service add-ons
- inventory and availability data
- bookings, guests, status history, and typed booking items
- coupons and discounts
- coupon redemptions
- payments, payment status history, and refunds
- webhook events
- reviews and moderation status
- webhook and audit-related operational records

### 7.2 Data Integrity Requirements

- all critical business entities must have stable internal identifiers
- public-facing entities should use non-sequential public identifiers where appropriate
- booking and payment state changes must be traceable
- inventory and booking relationships must preserve referential integrity
- deleted or archived operational data should not silently corrupt historical reporting
- valid lifecycle state transitions should be enforceable where feasible
- quantity, amount, and pricing fields must reject invalid negative values
- date and time ranges must reject invalid ordering

### 7.3 Data Retention Considerations

- booking and payment history should remain available for audit and support purposes
- review and moderation actions should be attributable
- partner approval decisions should remain traceable

## 8. Business Rules

- BR-001: a booking must not be confirmed until payment is trusted by the backend
- BR-002: a partner must not publish listings before approval
- BR-003: only eligible completed bookings may be reviewed
- BR-004: availability must reflect blackout periods and prior confirmed reservations
- BR-005: coupons must only apply when validation rules are satisfied
- BR-006: booking, payment, and refund transitions must follow allowed transition paths
- BR-007: only one review may exist per booking
- BR-008: partner-user permissions must align to partner access levels

## 9. Non-Functional Requirements

### 9.1 Performance

- NFR-PERF-001: normal search requests should respond within 2 to 3 seconds
- NFR-PERF-002: quote calculation should be fast enough to support interactive booking flows
- NFR-PERF-003: booking confirmation logic should avoid race conditions that lead to double booking

### 9.2 Reliability

- NFR-REL-001: payment webhook processing must be idempotent
- NFR-REL-002: booking and payment transitions must be resilient to transient failures
- NFR-REL-003: core operational data must be stored transactionally
- NFR-REL-004: provider event ingestion should be safely retryable without duplicate side effects

### 9.3 Security

- NFR-SEC-001: protected APIs must require authentication
- NFR-SEC-002: authorization must be role-aware
- NFR-SEC-003: secrets and credentials must not be hard-coded for shared environments
- NFR-SEC-004: raw payment card data must not be stored by the platform
- NFR-SEC-005: session tokens and recovery tokens should not be stored in plaintext

### 9.4 Maintainability

- NFR-MAIN-001: the system should remain organized as a modular monolith in early phases
- NFR-MAIN-002: backend code should follow feature-oriented boundaries
- NFR-MAIN-003: the monorepo should preserve clear separation between apps and shared packages

### 9.5 Usability

- NFR-USE-001: key user journeys should be understandable without training
- NFR-USE-002: booking, payment, and approval states should be clearly visible to the relevant user role

## 10. Constraints and Assumptions

- backend stack: ASP.NET Core / .NET 10
- frontend stack: Next.js / React / TypeScript
- database: PostgreSQL
- storage: S3-compatible object storage
- local development infrastructure may run through Docker Compose
- initial payment implementation may run in test mode only

## 11. Future Considerations

The following areas are intentionally deferred or expected to evolve later:

- loyalty programs
- referrals
- corporate booking features
- dynamic pricing
- richer analytics and reporting
- deeper transport integrations

## 12. Acceptance Criteria by Module

### 12.1 Authentication and Identity

Acceptance for this module is met when:

- users can register and log in successfully
- password reset requests and reset completion flows work end to end
- authenticated session state is created and validated correctly
- protected endpoints reject unauthorized access
- role-aware access checks can distinguish customer, partner, and admin capabilities

### 12.2 Partner and Property Management

Acceptance for this module is met when:

- partner onboarding can be submitted and reviewed
- approved partners can create and update properties
- property lifecycle states can move through expected approval and publication flows
- property media, policies, and amenities can be stored and retrieved

### 12.3 Offerings and Inventory

Acceptance for this module is met when:

- partners can create room, package, activity, venue, transport, and add-on records
- pricing and inventory data can be created and updated
- inventory constraints prevent impossible counts or invalid date or time ranges
- public discovery and quoting use current active offering data

### 12.4 Discovery and Booking

Acceptance for this module is met when:

- customers can search and filter public listings
- the system can generate quotes with discounts and fees
- bookings can be created for supported booking types
- booking records contain the expected ownership, guest, and item context
- invalid booking attempts are rejected when inventory is unavailable

### 12.5 Payments and Refunds

Acceptance for this module is met when:

- the backend can create payment records and provider-facing payment sessions
- payment state changes can be stored and tracked
- booking confirmation only occurs after trusted backend payment validation
- refund records and refund lifecycle handling are supported
- invalid payment or refund transitions are rejected

### 12.6 Webhooks and Reviews

Acceptance for this module is met when:

- webhook events are stored with processing status and deduplication behavior
- payment-related external events can be linked back to payments where applicable
- completed bookings can produce reviews
- review moderation states can be managed by admins
- review uniqueness by booking is enforced

## 13. Requirement Traceability by Module

### 13.1 IAM Module

- Authentication and identity: `FR-AUTH-001` to `FR-AUTH-010`
- User and role management: `FR-USER-001` to `FR-USER-006`
- Related schema domains: `iam`

### 13.2 Partner Module

- Partner onboarding: `FR-PARTNER-001` to `FR-PARTNER-005`
- Related schema domains: `core`, parts of `iam`

### 13.3 Catalog Module

- Property management: `FR-PROP-001` to `FR-PROP-008`
- Offering management: `FR-OFFER-001` to `FR-OFFER-012`
- Discovery: `FR-DISC-001` to `FR-DISC-005`
- Related schema domains: `catalog`

### 13.4 Inventory Module

- Inventory and pricing: `FR-INV-001` to `FR-INV-008`
- Related schema domains: `inventory`

### 13.5 Booking Module

- Quote and booking: `FR-BOOK-001` to `FR-BOOK-016`
- Coupons and discounts: `FR-COUPON-001` to `FR-COUPON-006`
- Related schema domains: `booking`

### 13.6 Billing Module

- Payments: `FR-PAY-001` to `FR-PAY-011`
- Related schema domains: `billing`

### 13.7 Integration Module

- Webhooks and external events: `FR-INT-001` to `FR-INT-004`
- Related schema domains: `integration`

### 13.8 Engagement Module

- Reviews: `FR-REV-001` to `FR-REV-007`
- Related schema domains: `engagement`

### 13.9 Administration Cross-Cut

- Administration: `FR-ADMIN-001` to `FR-ADMIN-003`
- Business rules: `BR-001` to `BR-008`
- Non-functional requirements: `NFR-PERF-001` to `NFR-USE-002`

## 14. Related Documents

- Monorepo docs: [README.md](README.md)
- Backend docs: [backend.md](backend.md)
- Frontend docs: [frontend.md](frontend.md)
- Packages docs: [packages.md](packages.md)
- Database draft: [../schema.sql](../schema.sql)
