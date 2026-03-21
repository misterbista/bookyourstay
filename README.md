

# 1. Product Vision

Build a platform where users can:

* book **hotels, resorts, staycations, and daycations**
* discover by **location, area, price, package, and activities**
* add **pick-up/drop-off** by road or air
* apply **discounts, referral rewards, and loyalty points**
* book **event spaces** for office gatherings, weddings, birthdays, exhibitions, conferences, and product launches
* request **decorations and add-on services**
* pay online and track booking/payment status
* leave **ratings, reviews, and comments**

This is basically a mix of:

* hotel marketplace
* experience marketplace
* event venue booking
* transport add-on platform
* promotion and loyalty engine

---

# 2. High-Level Product Modules

## 2.1 Customer Side

* user registration and login
* hotel/resort discovery
* search and filter
* staycation booking
* daycation booking
* package booking
* activity booking
* event venue booking
* decoration and service add-ons
* transport booking
* payment
* coupons / discounts
* loyalty and referral
* reviews and ratings
* booking history
* notifications

## 2.2 Hotel / Resort Partner Side

* partner onboarding
* hotel profile management
* room / package / event space management
* pricing and availability management
* transport service setup
* decoration/service offering management
* promotion management
* booking management
* refund/cancellation handling
* review response
* reporting/dashboard

## 2.3 Admin Side

* approve partners
* manage cities/areas/categories
* manage commission rates
* manage coupons and campaigns
* manage referral and loyalty rules
* dispute handling
* moderation of reviews/comments
* financial reconciliation
* analytics and reporting

---

# 3. Business Plan / System Plan

# 3.1 Core Offerings

## A. Accommodation

* hotel rooms
* resort villas
* suites
* cottages

## B. Staycation Offers

* overnight packages
* breakfast included
* dinner included
* pool access
* spa package
* romantic package
* family package

## C. Daycation Offers

* day pool access
* lunch buffet
* spa access
* day use room
* team outing day pass

## D. Event & Venue Booking

* wedding halls
* meeting rooms
* board rooms
* banquet halls
* conference halls
* exhibition spaces
* product launch venues
* outdoor lawns

## E. Add-On Services

* decoration packages
* sound and lighting
* DJ/music
* projector/screen
* catering
* photography
* flowers and balloons
* stage setup
* host/MC
* security
* event coordinator

## F. Adventure Activities

* zipline
* rafting
* paragliding
* jungle safari
* hiking
* ATV ride
* boating
* cycling
* bonfire
* cultural show

## G. Transport

* airport pick-up/drop
* helipad/air transport support
* private car
* bus/van
* jeep transfer
* shuttle services

---

# 3.2 User Flow

## Customer journey

1. User opens app/web
2. Searches by destination, date, event type, or property
3. Applies filters:

   * area/location
   * price
   * rating
   * activities
   * package type
   * event type
   * transport availability
4. Views hotel/resort details
5. Selects:

   * room/package/daycation/event venue
   * guests
   * activities
   * transport
   * decoration/services
6. Applies coupon/referral
7. Pays
8. Receives booking confirmation
9. Uses service
10. Gives rating/review
11. Earns loyalty points

---

# 3.3 Revenue Model

* commission per booking
* commission per event booking
* commission on add-ons
* premium listing for partners
* ads/promoted placements
* corporate subscription plan
* service fee on payment
* cancellation/rebooking fee share

---

# 4. SRS (Software Requirements Specification)

# 4.1 Introduction

## 4.1.1 Purpose

This system provides an online platform for customers to discover and book hotels, resorts, staycation offers, daycation offers, event venues, activities, transport, and event-related add-on services.

## 4.1.2 Scope

The system supports:

* accommodation booking
* daycation/staycation packages
* event bookings
* transport arrangements
* loyalty/referral systems
* coupon/discount management
* payment integration
* ratings/reviews/comments
* partner and admin management

## 4.1.3 Stakeholders

* customers
* hotel/resort partners
* corporate users
* event organizers
* platform admins
* payment gateway providers
* transport providers
* decorators/service vendors

---

# 4.2 Functional Requirements

## 4.2.1 User Management

The system shall:

* allow user sign-up/login via email, phone, OAuth
* support password reset
* support profile management
* support saved preferences
* support role-based access:

  * customer
  * partner
  * admin
  * corporate manager

## 4.2.2 Property Management

The system shall:

* allow partners to create hotel/resort profiles
* manage rooms, packages, amenities, policies, and images
* define supported booking types:

  * room booking
  * staycation
  * daycation
  * event venue booking
* define service area/location mapping
* define pricing calendars and inventory

## 4.2.3 Search and Filtering

The system shall allow search by:

* city
* area
* map location
* hotel/resort name
* property type
* event type

The system shall support filters:

* area/location
* price range
* rating
* amenities
* package type
* activity type
* transport availability
* event capacity
* indoor/outdoor
* family/couple/corporate friendly
* discount available
* loyalty eligible

## 4.2.4 Booking

The system shall support:

* room booking
* staycation package booking
* daycation package booking
* activity booking
* event venue booking
* service add-on booking
* transport booking

The system shall:

* validate availability
* calculate price
* calculate taxes and fees
* apply discounts
* reserve inventory
* confirm booking after successful payment or approved payment mode

## 4.2.5 Event Booking

The system shall:

* allow venue booking for:

  * office gathering
  * marriage
  * birthday
  * conference
  * exhibition
  * product launch
* allow selection of capacity, date, slot, duration
* allow add-ons like decoration, catering, AV setup
* allow custom event requests

## 4.2.6 Package Management

The system shall allow partners to define packages with:

* title
* category
* inclusions
* exclusions
* capacity
* schedule
* price
* offer validity
* blackout dates

## 4.2.7 Activities Management

The system shall:

* allow listing activities per hotel/resort
* manage activity schedule, price, capacity, and restrictions
* support bundled and standalone activity booking

## 4.2.8 Transport Management

The system shall support:

* road transfer booking
* air transfer inquiry/booking
* airport pick-up/drop
* one-way and round-trip
* vehicle type selection
* schedule and capacity
* price per route or package

## 4.2.9 Promotions / Discounts

The system shall support:

* coupons
* automatic discounts
* seasonal promotions
* first booking offers
* corporate offers
* referral rewards
* loyalty redemption

## 4.2.10 Loyalty Program

The system shall:

* award points per eligible booking
* allow redeeming points
* support tier levels:

  * silver
  * gold
  * platinum
* support expiry rules

## 4.2.11 Referral Program

The system shall:

* generate referral codes/links
* track invited users
* apply referral benefits
* prevent self-referral abuse

## 4.2.12 Ratings, Reviews, Comments

The system shall:

* allow verified users to rate completed bookings
* allow comments/reviews
* allow partner response
* allow moderation/reporting

## 4.2.13 Payment System

The system shall support:

* online payment gateway
* wallet/points redemption
* partial advance payment
* full payment
* refund handling
* payment status tracking

## 4.2.14 Notifications

The system shall send:

* booking confirmation
* cancellation/refund update
* event reminder
* pickup reminder
* coupon/referral reward update
* review request

## 4.2.15 Admin Controls

The system shall allow admins to:

* manage categories and locations
* approve or suspend partners
* manage featured listings
* manage disputes
* manage promotions
* view financial and operational reports

---

# 4.3 Non-Functional Requirements

## Performance

* search response under 2–3 seconds for standard queries
* payment confirmation handled reliably
* support concurrent bookings without inventory conflicts

## Scalability

* support multi-city and multi-country expansion
* horizontally scalable booking and search services

## Security

* encrypted passwords
* secure payment tokenization
* RBAC authorization
* audit logs
* fraud checks for referrals and payment abuse

## Availability

* target 99.9% uptime
* resilient payment retry and webhook handling

## Maintainability

* modular service boundaries
* API-first design
* clean domain model

## Usability

* mobile-friendly UI
* easy filter-based discovery
* clear checkout flow

---

# 4.4 User Roles

## Customer

* browse, book, pay, review

## Partner

* manage inventory, offers, availability, bookings

## Corporate User

* manage team/outing/event bookings

## Admin

* control system-wide operations

---

# 5. Suggested Architecture

Use a modular monolith first, then split to services when scale grows.

## Modules

* Identity & Access
* Property Management
* Inventory & Availability
* Search & Discovery
* Booking
* Pricing & Offers
* Event Management
* Activity Management
* Transport Management
* Payment
* Review & Rating
* Loyalty & Referral
* Notification
* Reporting/Admin

## Tech suggestion

* frontend: Next.js / React
* backend: .NET, Java, Node, or similar
* database: PostgreSQL
* cache: Redis
* search: PostgreSQL FTS first, Elasticsearch/OpenSearch later
* storage: S3-compatible object storage
* queue/events: RabbitMQ / Kafka / outbox pattern

---

# 6. Domain Model Overview

Main entities:

* User
* Role
* Hotel
* Resort
* Property
* RoomType
* Package
* Activity
* EventVenue
* TransportService
* DecorationService
* Booking
* BookingItem
* Payment
* Review
* Coupon
* LoyaltyAccount
* Referral
* Comment
* Offer
* Location
* Availability
* PricingRule

---

# 7. Schema Design

I recommend a **normalized PostgreSQL schema**.

---

# 7.1 Identity and Access

```sql
CREATE TABLE roles (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE users (
    id BIGSERIAL PRIMARY KEY,
    full_name VARCHAR(150) NOT NULL,
    email VARCHAR(150) UNIQUE,
    phone VARCHAR(30) UNIQUE,
    password_hash TEXT,
    status VARCHAR(30) NOT NULL DEFAULT 'active',
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE user_roles (
    user_id BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    role_id BIGINT NOT NULL REFERENCES roles(id) ON DELETE CASCADE,
    PRIMARY KEY (user_id, role_id)
);

CREATE TABLE user_addresses (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    label VARCHAR(50),
    country VARCHAR(100),
    state VARCHAR(100),
    city VARCHAR(100),
    area VARCHAR(100),
    address_line TEXT,
    latitude NUMERIC(10,7),
    longitude NUMERIC(10,7),
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);
```

---

# 7.2 Locations

```sql
CREATE TABLE cities (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    state_name VARCHAR(100),
    country_name VARCHAR(100) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE areas (
    id BIGSERIAL PRIMARY KEY,
    city_id BIGINT NOT NULL REFERENCES cities(id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    UNIQUE(city_id, name)
);
```

---

# 7.3 Partners and Properties

```sql
CREATE TABLE partners (
    id BIGSERIAL PRIMARY KEY,
    business_name VARCHAR(200) NOT NULL,
    contact_person_name VARCHAR(150),
    email VARCHAR(150),
    phone VARCHAR(30),
    status VARCHAR(30) NOT NULL DEFAULT 'pending',
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE properties (
    id BIGSERIAL PRIMARY KEY,
    partner_id BIGINT NOT NULL REFERENCES partners(id) ON DELETE RESTRICT,
    property_type VARCHAR(30) NOT NULL, -- hotel, resort, villa, lodge
    name VARCHAR(200) NOT NULL,
    description TEXT,
    city_id BIGINT REFERENCES cities(id),
    area_id BIGINT REFERENCES areas(id),
    address_line TEXT,
    latitude NUMERIC(10,7),
    longitude NUMERIC(10,7),
    star_rating INT,
    check_in_time TIME,
    check_out_time TIME,
    cancellation_policy TEXT,
    child_policy TEXT,
    pet_policy TEXT,
    status VARCHAR(30) NOT NULL DEFAULT 'draft',
    is_featured BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE property_images (
    id BIGSERIAL PRIMARY KEY,
    property_id BIGINT NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    image_url TEXT NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_cover BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE amenities (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    category VARCHAR(50)
);

CREATE TABLE property_amenities (
    property_id BIGINT NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    amenity_id BIGINT NOT NULL REFERENCES amenities(id) ON DELETE CASCADE,
    PRIMARY KEY (property_id, amenity_id)
);
```

---

# 7.4 Room Inventory

```sql
CREATE TABLE room_types (
    id BIGSERIAL PRIMARY KEY,
    property_id BIGINT NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    name VARCHAR(150) NOT NULL,
    description TEXT,
    capacity_adults INT NOT NULL,
    capacity_children INT NOT NULL DEFAULT 0,
    bed_type VARCHAR(100),
    room_size_sqft INT,
    total_inventory INT NOT NULL,
    base_price NUMERIC(12,2) NOT NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'active'
);

CREATE TABLE room_type_images (
    id BIGSERIAL PRIMARY KEY,
    room_type_id BIGINT NOT NULL REFERENCES room_types(id) ON DELETE CASCADE,
    image_url TEXT NOT NULL,
    sort_order INT NOT NULL DEFAULT 0
);

CREATE TABLE room_availability (
    id BIGSERIAL PRIMARY KEY,
    room_type_id BIGINT NOT NULL REFERENCES room_types(id) ON DELETE CASCADE,
    stay_date DATE NOT NULL,
    available_count INT NOT NULL,
    price_override NUMERIC(12,2),
    min_stay_nights INT,
    max_stay_nights INT,
    is_closed BOOLEAN NOT NULL DEFAULT FALSE,
    UNIQUE(room_type_id, stay_date)
);
```

---

# 7.5 Staycation and Daycation Packages

```sql
CREATE TABLE packages (
    id BIGSERIAL PRIMARY KEY,
    property_id BIGINT NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    package_type VARCHAR(30) NOT NULL, -- staycation, daycation, event, combo
    title VARCHAR(200) NOT NULL,
    description TEXT,
    inclusions TEXT,
    exclusions TEXT,
    terms_conditions TEXT,
    valid_from DATE,
    valid_to DATE,
    base_price NUMERIC(12,2) NOT NULL,
    max_guests INT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE package_availability (
    id BIGSERIAL PRIMARY KEY,
    package_id BIGINT NOT NULL REFERENCES packages(id) ON DELETE CASCADE,
    available_date DATE NOT NULL,
    start_time TIME,
    end_time TIME,
    available_count INT,
    price_override NUMERIC(12,2),
    is_closed BOOLEAN NOT NULL DEFAULT FALSE,
    UNIQUE(package_id, available_date, start_time, end_time)
);
```

---

# 7.6 Activities

```sql
CREATE TABLE activities (
    id BIGSERIAL PRIMARY KEY,
    property_id BIGINT NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    name VARCHAR(150) NOT NULL,
    activity_type VARCHAR(50), -- adventure, leisure, cultural
    description TEXT,
    duration_minutes INT,
    capacity INT,
    age_limit_min INT,
    age_limit_max INT,
    base_price NUMERIC(12,2) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE activity_slots (
    id BIGSERIAL PRIMARY KEY,
    activity_id BIGINT NOT NULL REFERENCES activities(id) ON DELETE CASCADE,
    slot_date DATE NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    available_count INT NOT NULL,
    price_override NUMERIC(12,2),
    UNIQUE(activity_id, slot_date, start_time, end_time)
);
```

---

# 7.7 Event Venues and Event Services

```sql
CREATE TABLE event_venues (
    id BIGSERIAL PRIMARY KEY,
    property_id BIGINT NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    name VARCHAR(150) NOT NULL,
    venue_type VARCHAR(50) NOT NULL, -- wedding_hall, conference_hall, lawn, exhibition_space
    description TEXT,
    indoor_outdoor VARCHAR(20),
    min_capacity INT,
    max_capacity INT,
    base_price NUMERIC(12,2) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE event_venue_slots (
    id BIGSERIAL PRIMARY KEY,
    event_venue_id BIGINT NOT NULL REFERENCES event_venues(id) ON DELETE CASCADE,
    event_date DATE NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    availability_status VARCHAR(30) NOT NULL DEFAULT 'available',
    price_override NUMERIC(12,2),
    UNIQUE(event_venue_id, event_date, start_time, end_time)
);

CREATE TABLE event_service_categories (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE -- decoration, catering, av, photography
);

CREATE TABLE event_services (
    id BIGSERIAL PRIMARY KEY,
    property_id BIGINT NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    category_id BIGINT NOT NULL REFERENCES event_service_categories(id),
    name VARCHAR(150) NOT NULL,
    description TEXT,
    pricing_type VARCHAR(30) NOT NULL, -- fixed, per_person, per_hour, per_event
    base_price NUMERIC(12,2) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);
```

---

# 7.8 Transport Services

```sql
CREATE TABLE transport_services (
    id BIGSERIAL PRIMARY KEY,
    property_id BIGINT NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    transport_mode VARCHAR(20) NOT NULL, -- road, air
    service_type VARCHAR(50) NOT NULL, -- pickup, drop, roundtrip, charter
    vehicle_type VARCHAR(50), -- car, jeep, van, bus, helicopter
    source_location VARCHAR(150),
    destination_location VARCHAR(150),
    capacity INT,
    base_price NUMERIC(12,2) NOT NULL,
    pricing_type VARCHAR(20) NOT NULL DEFAULT 'fixed',
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE transport_service_schedules (
    id BIGSERIAL PRIMARY KEY,
    transport_service_id BIGINT NOT NULL REFERENCES transport_services(id) ON DELETE CASCADE,
    service_date DATE NOT NULL,
    departure_time TIME,
    arrival_time TIME,
    available_count INT,
    price_override NUMERIC(12,2)
);
```

---

# 7.9 Booking Core

Use a single booking root and child items.

```sql
CREATE TABLE bookings (
    id BIGSERIAL PRIMARY KEY,
    booking_code VARCHAR(50) NOT NULL UNIQUE,
    user_id BIGINT NOT NULL REFERENCES users(id),
    property_id BIGINT REFERENCES properties(id),
    booking_type VARCHAR(30) NOT NULL, 
    -- accommodation, staycation, daycation, activity, event, mixed
    check_in_date DATE,
    check_out_date DATE,
    booking_date TIMESTAMP NOT NULL DEFAULT NOW(),
    guest_count_adults INT NOT NULL DEFAULT 1,
    guest_count_children INT NOT NULL DEFAULT 0,
    special_request TEXT,
    booking_status VARCHAR(30) NOT NULL DEFAULT 'pending',
    payment_status VARCHAR(30) NOT NULL DEFAULT 'pending',
    subtotal NUMERIC(12,2) NOT NULL DEFAULT 0,
    discount_amount NUMERIC(12,2) NOT NULL DEFAULT 0,
    tax_amount NUMERIC(12,2) NOT NULL DEFAULT 0,
    service_fee NUMERIC(12,2) NOT NULL DEFAULT 0,
    loyalty_redeemed_amount NUMERIC(12,2) NOT NULL DEFAULT 0,
    total_amount NUMERIC(12,2) NOT NULL DEFAULT 0,
    currency_code VARCHAR(10) NOT NULL DEFAULT 'NPR',
    coupon_id BIGINT,
    referral_code_used VARCHAR(50),
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE booking_items (
    id BIGSERIAL PRIMARY KEY,
    booking_id BIGINT NOT NULL REFERENCES bookings(id) ON DELETE CASCADE,
    item_type VARCHAR(30) NOT NULL, 
    -- room, package, activity, event_venue, event_service, transport
    reference_id BIGINT NOT NULL,
    title VARCHAR(200) NOT NULL,
    quantity INT NOT NULL DEFAULT 1,
    unit_price NUMERIC(12,2) NOT NULL,
    discount_amount NUMERIC(12,2) NOT NULL DEFAULT 0,
    tax_amount NUMERIC(12,2) NOT NULL DEFAULT 0,
    total_price NUMERIC(12,2) NOT NULL,
    meta_json JSONB
);
```

`reference_id` is polymorphic. In enterprise systems this is common, but if you want stronger integrity, split into separate tables like `booking_room_items`, `booking_activity_items`, etc.

---

# 7.10 Event Details

```sql
CREATE TABLE booking_events (
    id BIGSERIAL PRIMARY KEY,
    booking_id BIGINT NOT NULL UNIQUE REFERENCES bookings(id) ON DELETE CASCADE,
    event_type VARCHAR(50) NOT NULL, -- wedding, birthday, office_gathering, conference
    event_title VARCHAR(200),
    attendee_count INT,
    organizer_name VARCHAR(150),
    organizer_phone VARCHAR(30),
    organizer_email VARCHAR(150),
    start_datetime TIMESTAMP,
    end_datetime TIMESTAMP,
    notes TEXT
);
```

---

# 7.11 Guests

```sql
CREATE TABLE booking_guests (
    id BIGSERIAL PRIMARY KEY,
    booking_id BIGINT NOT NULL REFERENCES bookings(id) ON DELETE CASCADE,
    full_name VARCHAR(150) NOT NULL,
    age INT,
    gender VARCHAR(20),
    guest_type VARCHAR(20) NOT NULL DEFAULT 'adult'
);
```

---

# 7.12 Coupons, Discounts, Promotions

```sql
CREATE TABLE coupons (
    id BIGSERIAL PRIMARY KEY,
    code VARCHAR(50) NOT NULL UNIQUE,
    title VARCHAR(150) NOT NULL,
    description TEXT,
    discount_type VARCHAR(20) NOT NULL, -- percent, fixed
    discount_value NUMERIC(12,2) NOT NULL,
    min_order_amount NUMERIC(12,2),
    max_discount_amount NUMERIC(12,2),
    valid_from TIMESTAMP,
    valid_to TIMESTAMP,
    usage_limit_total INT,
    usage_limit_per_user INT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE coupon_redemptions (
    id BIGSERIAL PRIMARY KEY,
    coupon_id BIGINT NOT NULL REFERENCES coupons(id),
    user_id BIGINT NOT NULL REFERENCES users(id),
    booking_id BIGINT REFERENCES bookings(id),
    redeemed_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE promotions (
    id BIGSERIAL PRIMARY KEY,
    property_id BIGINT REFERENCES properties(id) ON DELETE CASCADE,
    title VARCHAR(150) NOT NULL,
    promo_type VARCHAR(30) NOT NULL, -- automatic, flash_sale, seasonal
    discount_type VARCHAR(20) NOT NULL,
    discount_value NUMERIC(12,2) NOT NULL,
    valid_from TIMESTAMP,
    valid_to TIMESTAMP,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    rules_json JSONB
);
```

---

# 7.13 Loyalty Program

```sql
CREATE TABLE loyalty_accounts (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL UNIQUE REFERENCES users(id) ON DELETE CASCADE,
    points_balance INT NOT NULL DEFAULT 0,
    tier_name VARCHAR(30) NOT NULL DEFAULT 'silver',
    lifetime_points INT NOT NULL DEFAULT 0,
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE loyalty_transactions (
    id BIGSERIAL PRIMARY KEY,
    loyalty_account_id BIGINT NOT NULL REFERENCES loyalty_accounts(id) ON DELETE CASCADE,
    booking_id BIGINT REFERENCES bookings(id),
    transaction_type VARCHAR(30) NOT NULL, -- earn, redeem, expire, adjust
    points INT NOT NULL,
    amount_value NUMERIC(12,2),
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);
```

---

# 7.14 Referral System

```sql
CREATE TABLE referrals (
    id BIGSERIAL PRIMARY KEY,
    referrer_user_id BIGINT NOT NULL REFERENCES users(id),
    referee_user_id BIGINT REFERENCES users(id),
    referral_code VARCHAR(50) NOT NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'pending', -- pending, qualified, rewarded, cancelled
    booking_id BIGINT REFERENCES bookings(id),
    reward_amount NUMERIC(12,2),
    reward_points INT,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);
```

---

# 7.15 Payments and Refunds

```sql
CREATE TABLE payments (
    id BIGSERIAL PRIMARY KEY,
    booking_id BIGINT NOT NULL REFERENCES bookings(id) ON DELETE CASCADE,
    payment_provider VARCHAR(50) NOT NULL,
    provider_transaction_id VARCHAR(150),
    payment_method VARCHAR(50) NOT NULL, -- card, wallet, bank, cash
    amount NUMERIC(12,2) NOT NULL,
    currency_code VARCHAR(10) NOT NULL DEFAULT 'NPR',
    payment_status VARCHAR(30) NOT NULL, -- pending, success, failed, refunded, partial_refund
    paid_at TIMESTAMP,
    raw_response_json JSONB,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE refunds (
    id BIGSERIAL PRIMARY KEY,
    payment_id BIGINT NOT NULL REFERENCES payments(id) ON DELETE CASCADE,
    refund_amount NUMERIC(12,2) NOT NULL,
    refund_reason TEXT,
    refund_status VARCHAR(30) NOT NULL,
    refunded_at TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);
```

---

# 7.16 Ratings, Reviews, Comments

```sql
CREATE TABLE reviews (
    id BIGSERIAL PRIMARY KEY,
    booking_id BIGINT NOT NULL REFERENCES bookings(id) ON DELETE CASCADE,
    user_id BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    property_id BIGINT NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    rating_overall NUMERIC(2,1) NOT NULL,
    rating_cleanliness NUMERIC(2,1),
    rating_service NUMERIC(2,1),
    rating_location NUMERIC(2,1),
    title VARCHAR(150),
    comment TEXT,
    status VARCHAR(30) NOT NULL DEFAULT 'published',
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    UNIQUE(booking_id, user_id)
);

CREATE TABLE review_comments (
    id BIGSERIAL PRIMARY KEY,
    review_id BIGINT NOT NULL REFERENCES reviews(id) ON DELETE CASCADE,
    user_id BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    comment TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);
```

---

# 7.17 Wishlist / Favorites

```sql
CREATE TABLE favorites (
    user_id BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    property_id BIGINT NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (user_id, property_id)
);
```

---

# 7.18 Notifications

```sql
CREATE TABLE notifications (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    channel VARCHAR(20) NOT NULL, -- email, sms, push, inapp
    title VARCHAR(150) NOT NULL,
    message TEXT NOT NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'pending',
    sent_at TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);
```

---

# 7.19 Audit Logs

```sql
CREATE TABLE audit_logs (
    id BIGSERIAL PRIMARY KEY,
    actor_user_id BIGINT REFERENCES users(id),
    entity_name VARCHAR(100) NOT NULL,
    entity_id BIGINT,
    action VARCHAR(50) NOT NULL,
    old_values JSONB,
    new_values JSONB,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);
```

---

# 8. Recommended Stronger Schema Option for Booking Items

For robustness, instead of one polymorphic `booking_items`, you can use:

* `booking_room_items`
* `booking_package_items`
* `booking_activity_items`
* `booking_event_venue_items`
* `booking_event_service_items`
* `booking_transport_items`

This gives:

* stronger foreign key integrity
* better reporting
* easier validation

For a serious production system, I recommend this stronger option.

---

# 9. Important Business Rules

## Availability

* room cannot be overbooked
* event venue slot cannot be double-booked
* activity slot cannot exceed capacity
* transport schedule cannot exceed available seats/units

## Booking Status

Suggested values:

* pending
* reserved
* confirmed
* cancelled
* completed
* no_show
* expired

## Payment Status

* pending
* authorized
* paid
* failed
* partially_refunded
* refunded

## Review Rules

* only completed bookings can review
* one main review per booking per user
* partner can reply, not alter customer review

## Referral Rules

* referrer rewarded only after referee’s first completed eligible booking
* self-referral blocked
* suspicious account/device patterns flagged

## Loyalty Rules

* points awarded after completion, not just payment
* redemption cannot exceed configured percent of order
* points may expire after set time

---

# 10. Search and Filter Design

To support your required filters, index these:

## Main searchable/filterable fields

* property city_id
* property area_id
* property_type
* room/package base price
* rating average
* event venue capacity
* transport mode
* activity type
* promotion active window

## Suggested indexes

```sql
CREATE INDEX idx_properties_city_area ON properties(city_id, area_id);
CREATE INDEX idx_properties_type_status ON properties(property_type, status);
CREATE INDEX idx_room_availability_date ON room_availability(room_type_id, stay_date);
CREATE INDEX idx_package_availability_date ON package_availability(package_id, available_date);
CREATE INDEX idx_activity_slots_date ON activity_slots(activity_id, slot_date);
CREATE INDEX idx_event_venue_slots_date ON event_venue_slots(event_venue_id, event_date);
CREATE INDEX idx_bookings_user_status ON bookings(user_id, booking_status);
CREATE INDEX idx_reviews_property ON reviews(property_id);
```

For text search:

```sql
CREATE INDEX idx_properties_name_search 
ON properties USING GIN (to_tsvector('english', name || ' ' || COALESCE(description,'')));
```

---

# 11. API Design Suggestion

## Public APIs

* `POST /auth/register`
* `POST /auth/login`
* `GET /properties`
* `GET /properties/{id}`
* `GET /properties/{id}/availability`
* `GET /packages`
* `GET /activities`
* `GET /event-venues`
* `POST /bookings`
* `GET /bookings/{id}`
* `POST /payments/initiate`
* `POST /payments/webhook`
* `POST /reviews`
* `POST /referrals/apply`
* `GET /loyalty/account`

## Partner APIs

* `POST /partner/properties`
* `POST /partner/room-types`
* `POST /partner/packages`
* `POST /partner/activities`
* `POST /partner/event-venues`
* `POST /partner/event-services`
* `POST /partner/transport-services`
* `GET /partner/bookings`

## Admin APIs

* `GET /admin/partners`
* `PATCH /admin/partners/{id}/approve`
* `POST /admin/coupons`
* `POST /admin/promotions`
* `GET /admin/reports/revenue`

---

# 12. Use Cases

## Use Case 1: Staycation booking

* user selects hotel
* chooses date and room/package
* adds airport pick-up
* applies coupon
* pays
* booking confirmed

## Use Case 2: Daycation office outing

* corporate user selects resort
* chooses daycation package
* adds lunch + activities + bus transport
* pays deposit
* receives booking confirmation

## Use Case 3: Wedding event

* user selects venue
* chooses date/time slot
* adds decoration, catering, photography
* confirms attendee estimate
* pays advance
* venue reserved

## Use Case 4: Product launch

* company selects conference/event hall
* adds stage, projector, branding, refreshments
* books transport for VIP guests
* confirms service package

---

# 13. MVP Scope

For MVP, build these first:

## Phase 1 MVP

* user auth
* property listing
* search/filter by area and price
* room/staycation/daycation booking
* coupons
* payment
* reviews
* partner dashboard basic

## Phase 2

* activities
* transport
* loyalty
* referral
* event venue booking

## Phase 3

* decoration/services
* corporate booking workflows
* advanced analytics
* dynamic pricing
* vendor marketplace

---

# 14. Risks and Challenges

* inventory consistency under concurrent booking
* cancellation/refund complexity
* event bookings need custom workflows
* transport and air services may need approval/manual confirmation
* fraud in coupons/referrals
* seasonality and dynamic pricing complexity
* many add-ons can make checkout messy

---

# 15. Recommended Design Decisions

## Best design choices

* PostgreSQL as primary DB
* modular monolith first
* separate inventory tables per resource type
* event-driven payment confirmation
* separate pricing engine
* soft delete/audit logs for partner/admin changes
* verified review model
* configurable loyalty/referral rules in admin

---

# 16. Final Recommendation on Schema Style

For production, use:

* **core normalized tables**
* **separate availability tables**
* **separate booking item tables per resource type**
* **JSONB only for flexible metadata**, not core transactional data

That gives you:

* strong data integrity
* better reporting
* easier maintenance
* easier scaling later

---

I can next turn this into a more formal **IEEE-style SRS document**, or generate a **full ERD and PostgreSQL SQL script**.
