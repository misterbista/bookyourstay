# BookYourStay

## Overview

BookYourStay is a booking platform for:

* hotels and resorts
* staycations and daycations
* event venues
* activities
* transport add-ons
* service add-ons like decoration or catering

The goal is to let customers discover a property, select a bookable offering, add extras, pay online, and track the booking from confirmation to completion.

This document is written as an MVP product and API guide. It focuses on the features to build first, the main use cases, and the API endpoints needed to support them.

## MVP Scope

### Included

* customer sign up and login
* partner onboarding and approval
* property listing and management
* room, package, activity, event venue, and transport setup
* search and filtering
* booking and checkout
* coupon support
* dummy Stripe payment flow
* booking status tracking
* reviews and ratings
* admin approval and moderation

### Deferred

* loyalty points
* referral rewards
* corporate booking workflows
* advanced analytics
* dynamic pricing engine
* real third-party transport integrations

## Users and Roles

### Customer

* search and view properties
* create bookings
* add transport and service add-ons
* pay online
* cancel eligible bookings
* review completed bookings

### Partner

* create and manage properties
* manage inventory, pricing, and availability
* manage bookings and cancellation requests
* define activities, event spaces, transport, and add-ons

### Admin

* approve partners and properties
* manage categories, locations, and coupons
* monitor bookings and payments
* moderate reviews
* resolve disputes

## Core Booking Types

The platform supports these bookable products:

* room booking
* staycation package
* daycation package
* activity booking
* event venue booking

Optional add-ons:

* road transport
* air transport inquiry
* decoration package
* catering
* AV setup
* photography
* other partner-defined services

## Main Use Cases

### 1. Customer books a room or package

1. Customer searches by location and date.
2. Customer filters by price, rating, amenities, and property type.
3. Customer opens a property detail page.
4. Customer selects a room, staycation, or daycation option.
5. System checks availability and calculates price.
6. Customer adds coupon and optional add-ons.
7. Customer proceeds to payment.
8. System creates a pending booking and pending payment.
9. Dummy Stripe payment succeeds.
10. Booking is confirmed and inventory is reserved.

### 2. Customer books an event venue

1. Customer selects event type, guest count, date, and duration.
2. System shows available venues and packages.
3. Customer adds decoration, catering, and AV services.
4. System calculates total price.
5. Customer pays online.
6. Booking is confirmed after successful payment.

### 3. Customer adds transport

1. Customer selects transport during checkout.
2. Customer chooses one-way or round trip.
3. System prices the selected vehicle or route.
4. Transport is attached to the booking as an add-on item.

### 4. Partner manages availability

1. Partner creates room types, packages, activities, or event spaces.
2. Partner updates inventory, capacity, blackout dates, and price.
3. Search results and booking availability reflect these updates.

### 5. Admin approves a partner

1. Partner registers and submits business details.
2. Admin reviews the application.
3. Admin approves or rejects the partner.
4. Only approved partners can publish listings.

## Dummy Stripe Payment

The system will use Stripe in test mode only. This means:

* no real money is charged
* all payments are simulated using Stripe test credentials
* booking confirmation depends on successful test payment

### Recommended MVP Approach

Use Stripe Payment Intents in test mode.

Flow:

1. Backend creates a booking in `pending_payment` state.
2. Backend creates a Stripe test Payment Intent.
3. Frontend confirms the payment using Stripe.js and test card data.
4. Backend receives Stripe webhook event.
5. Backend marks payment as `paid`.
6. Backend marks booking as `confirmed`.

### Payment States

* `pending`
* `requires_action`
* `paid`
* `failed`
* `refunded`
* `partially_refunded`

### Booking States

* `draft`
* `pending_payment`
* `confirmed`
* `cancelled`
* `completed`
* `refund_pending`
* `refunded`

### Stripe Webhooks to Handle

* `payment_intent.succeeded`
* `payment_intent.payment_failed`
* `charge.refunded`

### Important Rule

Do not mark a booking as confirmed only from the frontend response. Confirm it after the backend validates the Stripe webhook or verifies the Payment Intent status.

## Functional Requirements

### Authentication

* email and password sign up
* login and logout
* password reset
* role-based access control

### Discovery

* search by city, area, date, property name, and booking type
* filter by price, rating, amenities, capacity, and availability
* sort by price, rating, and popularity

### Property Management

* partner creates property profile
* partner uploads images and amenities
* partner defines room types, packages, event spaces, activities, and transport options
* partner manages pricing and availability

### Booking

* price calculation
* coupon application
* tax and fee calculation
* inventory validation
* booking creation
* cancellation and refund tracking

### Reviews

* only completed bookings can be reviewed
* review includes rating and comment
* admin can hide abusive content

## Suggested System Modules

* Auth
* Users
* Partners
* Properties
* Inventory
* Search
* Pricing
* Bookings
* Payments
* Reviews
* Coupons
* Notifications
* Admin

## API Conventions

### Base Path

`/api/v1`

### Auth

Use bearer token authentication for protected endpoints.

### Common Response Shape

```json
{
  "success": true,
  "message": "Request completed",
  "data": {},
  "meta": {}
}
```

### Common Error Shape

```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "field": ["error message"]
  }
}
```

## API Endpoints

### Authentication

* `POST /api/v1/auth/register` - register customer
* `POST /api/v1/auth/login` - login user
* `POST /api/v1/auth/logout` - logout user
* `POST /api/v1/auth/forgot-password` - request password reset
* `POST /api/v1/auth/reset-password` - reset password
* `GET /api/v1/auth/me` - get current user profile

### Customer Profile

* `GET /api/v1/users/me` - get profile
* `PATCH /api/v1/users/me` - update profile
* `GET /api/v1/users/me/bookings` - list customer bookings
* `GET /api/v1/users/me/payments` - list customer payments
* `GET /api/v1/users/me/reviews` - list customer reviews

### Partner Onboarding

* `POST /api/v1/partners/apply` - submit partner application
* `GET /api/v1/partners/me` - get partner profile
* `PATCH /api/v1/partners/me` - update partner profile
* `GET /api/v1/partners/me/status` - get approval status

### Properties

* `GET /api/v1/properties` - public property list
* `GET /api/v1/properties/{propertyId}` - public property detail
* `POST /api/v1/partner/properties` - create property
* `GET /api/v1/partner/properties` - list partner properties
* `GET /api/v1/partner/properties/{propertyId}` - get partner property detail
* `PATCH /api/v1/partner/properties/{propertyId}` - update property
* `DELETE /api/v1/partner/properties/{propertyId}` - archive property

### Room Types

* `POST /api/v1/partner/properties/{propertyId}/rooms` - create room type
* `GET /api/v1/partner/properties/{propertyId}/rooms` - list room types
* `PATCH /api/v1/partner/rooms/{roomId}` - update room type
* `DELETE /api/v1/partner/rooms/{roomId}` - archive room type

### Packages

* `POST /api/v1/partner/properties/{propertyId}/packages` - create staycation or daycation package
* `GET /api/v1/properties/{propertyId}/packages` - list public packages
* `PATCH /api/v1/partner/packages/{packageId}` - update package
* `DELETE /api/v1/partner/packages/{packageId}` - archive package

### Activities

* `POST /api/v1/partner/properties/{propertyId}/activities` - create activity
* `GET /api/v1/properties/{propertyId}/activities` - list public activities
* `PATCH /api/v1/partner/activities/{activityId}` - update activity
* `DELETE /api/v1/partner/activities/{activityId}` - archive activity

### Event Venues

* `POST /api/v1/partner/properties/{propertyId}/venues` - create event venue
* `GET /api/v1/properties/{propertyId}/venues` - list public event venues
* `PATCH /api/v1/partner/venues/{venueId}` - update event venue
* `DELETE /api/v1/partner/venues/{venueId}` - archive event venue

### Transport and Add-Ons

* `POST /api/v1/partner/properties/{propertyId}/transport-options` - create transport option
* `GET /api/v1/properties/{propertyId}/transport-options` - list public transport options
* `POST /api/v1/partner/properties/{propertyId}/addons` - create service add-on
* `GET /api/v1/properties/{propertyId}/addons` - list public add-ons
* `PATCH /api/v1/partner/transport-options/{transportId}` - update transport option
* `PATCH /api/v1/partner/addons/{addonId}` - update service add-on

### Availability and Pricing

* `GET /api/v1/properties/{propertyId}/availability` - public availability check
* `POST /api/v1/partner/inventory/bulk-update` - bulk inventory update
* `POST /api/v1/partner/pricing/bulk-update` - bulk pricing update
* `POST /api/v1/bookings/quote` - calculate booking price before creating booking

### Search and Filters

* `GET /api/v1/search/properties` - search properties
* `GET /api/v1/search/suggestions` - search suggestions
* `GET /api/v1/locations` - list cities and areas
* `GET /api/v1/categories` - list booking categories

### Coupons

* `POST /api/v1/coupons/validate` - validate coupon code
* `POST /api/v1/admin/coupons` - create coupon
* `GET /api/v1/admin/coupons` - list coupons
* `PATCH /api/v1/admin/coupons/{couponId}` - update coupon

### Bookings

* `POST /api/v1/bookings` - create booking
* `GET /api/v1/bookings/{bookingId}` - get booking detail
* `POST /api/v1/bookings/{bookingId}/cancel` - cancel booking
* `GET /api/v1/partner/bookings` - partner booking list
* `GET /api/v1/partner/bookings/{bookingId}` - partner booking detail
* `PATCH /api/v1/partner/bookings/{bookingId}` - update booking status or notes

### Payments

* `POST /api/v1/payments/create-intent` - create Stripe test Payment Intent
* `POST /api/v1/payments/{bookingId}/confirm` - confirm payment status from backend
* `GET /api/v1/payments/{paymentId}` - get payment detail
* `POST /api/v1/payments/{paymentId}/refund` - request refund
* `POST /api/v1/webhooks/stripe` - Stripe webhook receiver

### Reviews

* `POST /api/v1/bookings/{bookingId}/reviews` - create review
* `GET /api/v1/properties/{propertyId}/reviews` - list public reviews
* `PATCH /api/v1/reviews/{reviewId}` - edit own review
* `DELETE /api/v1/reviews/{reviewId}` - delete own review

### Admin

* `GET /api/v1/admin/partners` - list partner applications
* `PATCH /api/v1/admin/partners/{partnerId}/approve` - approve partner
* `PATCH /api/v1/admin/partners/{partnerId}/reject` - reject partner
* `GET /api/v1/admin/properties` - list all properties
* `PATCH /api/v1/admin/properties/{propertyId}/publish` - publish property
* `PATCH /api/v1/admin/properties/{propertyId}/unpublish` - unpublish property
* `GET /api/v1/admin/bookings` - list all bookings
* `GET /api/v1/admin/payments` - list all payments
* `GET /api/v1/admin/reviews` - list all reviews
* `PATCH /api/v1/admin/reviews/{reviewId}/hide` - hide review

## Key Request Payloads

### Create Booking

`POST /api/v1/bookings`

```json
{
  "propertyId": "prop_123",
  "bookingType": "staycation",
  "itemId": "pkg_123",
  "checkIn": "2026-04-10",
  "checkOut": "2026-04-11",
  "guests": {
    "adults": 2,
    "children": 1
  },
  "addons": [
    {
      "type": "transport",
      "id": "trp_001",
      "quantity": 1
    },
    {
      "type": "service",
      "id": "add_001",
      "quantity": 1
    }
  ],
  "couponCode": "NEWUSER10",
  "specialRequest": "Late check-in"
}
```

### Create Payment Intent

`POST /api/v1/payments/create-intent`

```json
{
  "bookingId": "bok_123",
  "currency": "USD"
}
```

Response:

```json
{
  "success": true,
  "data": {
    "paymentId": "pay_123",
    "clientSecret": "pi_test_secret",
    "publishableKey": "pk_test_xxx",
    "amount": 12000,
    "currency": "usd",
    "status": "pending"
  }
}
```

## Booking and Payment Flow

### Standard Checkout Flow

1. Customer requests a quote using `POST /api/v1/bookings/quote`.
2. Customer creates a booking using `POST /api/v1/bookings`.
3. Backend stores booking as `pending_payment`.
4. Frontend requests Stripe test Payment Intent using `POST /api/v1/payments/create-intent`.
5. Frontend confirms card payment using Stripe.js.
6. Stripe sends webhook to `POST /api/v1/webhooks/stripe`.
7. Backend verifies the event and updates payment state.
8. Backend marks the booking as `confirmed`.

### Cancel and Refund Flow

1. Customer sends `POST /api/v1/bookings/{bookingId}/cancel`.
2. Backend checks cancellation policy.
3. If refundable, backend creates refund request.
4. Payment status becomes `refund_pending`.
5. After refund succeeds, payment becomes `refunded`.
6. Booking becomes `cancelled` or `refunded`, depending on reporting rules.

## Minimal Data Model

Main entities:

* User
* Partner
* Property
* Room
* Package
* Activity
* Venue
* TransportOption
* Addon
* Booking
* BookingItem
* Payment
* Coupon
* Review

## Database Schema

`schema.sql` is the single source of truth for the production-grade database design:

* SQL Server aligned with `.Net 10`
* mixed auth support with local auth plus Google/social identities
* JWT refresh-session storage
* API-driven geography
* typed booking item tables
* Stripe-compatible payments, refunds, and webhook tracking
* audit/history tables for booking and payment state changes

See [schema.sql](/Users/maccy/bookyourstay/schema.sql) for the full schema.

### Schema Summary by Domain

* `iam` - users, identities, sessions, roles, addresses
* `core` - partners and partner users
* `catalog` - properties, room types, packages, activities, venues, transport, add-ons
* `inventory` - room, package, activity, and venue availability
* `booking` - bookings, typed booking items, coupons, booking history
* `billing` - payments, refunds, payment history
* `integration` - webhook events
* `engagement` - reviews

## Suggested Tech Stack

* frontend: Next.js
* backend: .Net 10
* database: SQL Server
* cache: Redis
* object storage: S3-compatible storage
* payments: Stripe test mode

## Non-Functional Requirements

* search should respond within 2 to 3 seconds for normal queries
* booking creation must prevent double booking
* payment webhooks must be idempotent
* protected APIs must use RBAC
* all booking and payment state changes must be logged

## Delivery Plan

### Phase 1

* auth
* partner onboarding
* property management
* room and package booking
* search and filters
* coupons
* dummy Stripe payment
* booking history
* reviews

### Phase 2

* activities
* event venues
* transport add-ons
* service add-ons
* admin reporting

### Phase 3

* loyalty
* referrals
* corporate bookings
* advanced pricing and analytics

## Final Notes

This product should be built as a modular monolith first. Keep booking, inventory, pricing, and payment logic separated into clear modules, but avoid early microservices. The highest-risk area is the booking and payment flow, so inventory locking, webhook verification, and status transitions should be implemented carefully from the start.
