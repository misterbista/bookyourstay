# Database

This document explains the current database design at a higher level than `schema.sql`.

## Overview

The database is designed as a modular PostgreSQL schema set for a booking platform. It uses:

- separate PostgreSQL schemas for major domains
- BIGINT identity keys for internal relations
- UUID public identifiers for external-facing entities
- explicit reference tables instead of native enums
- audit-oriented status history and transition rules

The implementation-oriented source of truth remains [../schema.sql](../schema.sql).

## Domain Schemas

### `iam`

Identity and access management:

- users
- user identities
- user sessions
- verification and reset tokens
- roles
- user-role assignments
- user addresses

This domain supports local auth, Google auth, phone OTP, hashed session tokens, and role-based access control.

### `core`

Partner organization data:

- partners
- partner users

This domain models the business account itself and the relationship between a partner organization and multiple operational users with access levels.

### `catalog`

Public and partner-managed listing content:

- properties
- property images, policies, and amenities
- room types and room images
- packages
- activities
- event venues
- transport options
- service add-ons

This is the main content and commercial catalog layer of the platform.

### `inventory`

Availability and sellable capacity:

- room inventory by date
- package inventory by date
- activity slots
- venue slots

This domain supports both date-based inventory and time-slot-based inventory.

### `booking`

Booking and booking-related commerce:

- coupons
- bookings
- coupon redemptions
- booking status history
- booking guests
- typed booking item tables

The schema uses separate booking item tables for each sellable type instead of one generic polymorphic table.

### `billing`

Payments and refunds:

- payments
- payment status history
- refunds

This domain supports provider-aware payment records, refund workflows, and state transition tracking.

### `integration`

External event intake:

- webhook events

This domain stores provider events, processing state, related payment linkage, and error information for idempotent webhook handling.

### `engagement`

Customer feedback:

- reviews

Reviews are linked back to bookings, customers, and properties and support moderation-related statuses.

## Reference Data Strategy

The schema uses reference tables for flexible business categories and states, including:

- user statuses
- identity providers
- partner access levels
- partner statuses
- property types and property statuses
- package types
- venue placements
- transport types and trip types
- add-on pricing models
- slot statuses
- booking types and booking statuses
- discount types
- payment providers and payment method types
- payment and refund statuses
- webhook processing statuses
- review statuses

This keeps business rules explicit and easier to evolve than hard-coded enums.

## Core Entity Relationships

### Users and Auth

- a user can have many identities
- a user can have many sessions
- a user can have many roles
- a user can have many addresses

### Partners and Properties

- a partner can have many partner users
- a partner can have many properties
- a property belongs to exactly one partner

### Offerings

- a property can have many room types
- a property can have many packages
- a property can have many activities
- a property can have many event venues
- a property can have many transport options
- a property can have many service add-ons

### Inventory

- room inventory belongs to a room type and date
- package inventory belongs to a package and date
- activity slots belong to an activity
- venue slots belong to a venue

### Bookings

- a booking belongs to one customer, one property, and one partner
- a booking may reference one coupon
- a booking may have many guests
- a booking may have one or more typed item rows depending on booking composition

### Billing

- a payment belongs to one booking
- a refund belongs to one payment
- payment and booking status history are stored separately from the current state

### Reviews

- a review is tied to one booking
- a review is tied to one customer and one property
- the schema enforces one review per booking

## Booking Model

The booking system is intentionally typed.

Instead of a single `booking_items` table with many nullable columns, the schema currently uses:

- `booking.booking_room_items`
- `booking.booking_package_items`
- `booking.booking_activity_items`
- `booking.booking_venue_items`
- `booking.booking_transport_items`
- `booking.booking_addon_items`

This makes constraints and downstream reporting more explicit.

## Inventory Model

Two inventory styles are modeled:

- date-based inventory for rooms and packages
- time-slot-based inventory for activities and venues

Room inventory additionally supports:

- reserved inventory
- out-of-service inventory
- price overrides
- minimum and maximum stay rules
- closed-to-arrival and closed-to-departure flags

## Status and Workflow Model

The schema does more than store statuses. It also models allowed transitions using transition tables:

- `booking.booking_status_transitions`
- `billing.payment_status_transitions`
- `billing.refund_status_transitions`

Trigger functions enforce those transitions when status fields are updated. This means invalid lifecycle jumps can be rejected at the database layer.

## Audit and Traceability

Audit-oriented structures currently include:

- booking status history
- payment status history
- webhook event storage
- approval timestamps and approver references
- created and updated timestamps across key tables

There is also an `updated_at` trigger installed for many mutable tables.

## Pricing and Money

Money-related columns generally use:

- `NUMERIC(14,2)` for amounts
- `CHAR(3)` for currencies

The schema models:

- base prices on sellable offerings
- per-date or per-slot price overrides
- booking subtotal, discount, tax, service fee, total, and amount due
- payment amounts
- refund amounts

## Payment and Webhook Design

The billing and integration design anticipates external processors such as Stripe:

- payments store provider payment intent and charge references
- refunds store provider refund references
- webhook events are stored with provider event IDs
- idempotency is supported through uniqueness constraints and event deduplication

This is consistent with a backend-validated payment flow rather than trusting the frontend alone.

## Review Model

Reviews are tied directly to the completed booking context and support:

- rating
- title
- body
- partner response
- moderation states such as published, hidden, and flagged

## Constraints and Validation Patterns

The schema includes many integrity rules, such as:

- non-negative money amounts
- valid date and time ordering
- occupancy and capacity checks
- latitude and longitude bounds
- unique public IDs and business keys
- uniqueness rules for coupon use, reviews, and provider identifiers

## Seeds and Operational Defaults

The schema seeds important operational data for:

- statuses
- transition rules
- role definitions
- offering classifications
- payment classifications

This gives the application a stable business baseline from first startup.

## Relationship to Backend Migrations

There are currently two database concepts in the repo:

- [../schema.sql](../schema.sql): broader production-oriented schema draft
- [../apps/backend/Database/Migrations](../apps/backend/Database/Migrations): backend runtime migrations currently focused on implemented backend slices

The long-term direction should be to keep these aligned so the documentation, schema draft, and runtime migrations describe the same system over time.
