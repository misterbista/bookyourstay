/*
Production-grade PostgreSQL schema for BookYourStay

Targets:
- PostgreSQL 15+
- .NET 10 backend
- Stripe test/live integration without storing raw card data
- Modular monolith with strong referential integrity

Notes:
- Uses dedicated schemas to keep the domain organized
- Uses BIGINT generated identities for internal keys plus UUID public IDs
- Uses timestamptz for all timestamps
- Uses flexible reference tables instead of native PostgreSQL enums
- Uses partial unique indexes for nullable uniqueness
*/

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

CREATE SCHEMA IF NOT EXISTS core;
CREATE SCHEMA IF NOT EXISTS iam;
CREATE SCHEMA IF NOT EXISTS catalog;
CREATE SCHEMA IF NOT EXISTS inventory;
CREATE SCHEMA IF NOT EXISTS booking;
CREATE SCHEMA IF NOT EXISTS billing;
CREATE SCHEMA IF NOT EXISTS engagement;
CREATE SCHEMA IF NOT EXISTS integration;

/* =========================
   Flexible reference data
   ========================= */

/* IAM reference data */

CREATE TABLE iam.user_statuses (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE iam.identity_providers (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE iam.partner_access_levels (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE core.partner_statuses (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

/* Catalog reference data */

CREATE TABLE catalog.property_types (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE catalog.property_statuses (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE catalog.policy_types (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE catalog.package_types (
    code VARCHAR(20) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE catalog.venue_placements (
    code VARCHAR(20) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE catalog.transport_types (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE catalog.trip_types (
    code VARCHAR(20) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE catalog.addon_pricing_models (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE inventory.slot_statuses (
    code VARCHAR(20) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

/* Booking reference data */

CREATE TABLE inventory.venue_slot_statuses (
    code VARCHAR(20) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE booking.discount_types (
    code VARCHAR(20) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE booking.booking_types (
    code VARCHAR(20) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE booking.booking_statuses (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE booking.booking_status_transitions (
    from_status VARCHAR(30) NOT NULL REFERENCES booking.booking_statuses(code) ON DELETE CASCADE,
    to_status VARCHAR(30) NOT NULL REFERENCES booking.booking_statuses(code) ON DELETE CASCADE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (from_status, to_status)
);

CREATE TABLE booking.guest_types (
    code VARCHAR(20) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE billing.payment_providers (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

/* Billing reference data */

CREATE TABLE billing.payment_method_types (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE billing.payment_statuses (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE billing.payment_status_transitions (
    from_status VARCHAR(30) NOT NULL REFERENCES billing.payment_statuses(code) ON DELETE CASCADE,
    to_status VARCHAR(30) NOT NULL REFERENCES billing.payment_statuses(code) ON DELETE CASCADE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (from_status, to_status)
);

CREATE TABLE billing.refund_statuses (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE billing.refund_status_transitions (
    from_status VARCHAR(30) NOT NULL REFERENCES billing.refund_statuses(code) ON DELETE CASCADE,
    to_status VARCHAR(30) NOT NULL REFERENCES billing.refund_statuses(code) ON DELETE CASCADE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (from_status, to_status)
);

CREATE TABLE integration.webhook_processing_statuses (
    code VARCHAR(20) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

/* Engagement reference data */

CREATE TABLE engagement.review_statuses (
    code VARCHAR(20) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

/* =========================
   IAM
   ========================= */

CREATE TABLE iam.roles (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    code VARCHAR(50) NOT NULL,
    name VARCHAR(100) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_roles_public_id UNIQUE (public_id),
    CONSTRAINT uq_roles_code UNIQUE (code)
);

CREATE TABLE iam.users (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    full_name VARCHAR(150) NOT NULL,
    email CITEXT NULL,
    phone VARCHAR(30) NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'active',
    email_verified_at TIMESTAMPTZ NULL,
    phone_verified_at TIMESTAMPTZ NULL,
    last_login_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at TIMESTAMPTZ NULL,
    CONSTRAINT uq_users_public_id UNIQUE (public_id),
    CONSTRAINT fk_users_status FOREIGN KEY (status) REFERENCES iam.user_statuses(code),
    CONSTRAINT ck_users_contact CHECK (email IS NOT NULL OR phone IS NOT NULL)
);

CREATE TABLE iam.user_identities (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id BIGINT NOT NULL REFERENCES iam.users(id) ON DELETE CASCADE,
    provider VARCHAR(30) NOT NULL,
    provider_subject VARCHAR(200) NOT NULL,
    password_hash VARCHAR(500) NULL,
    provider_email CITEXT NULL,
    provider_metadata JSONB NULL,
    verified_at TIMESTAMPTZ NULL,
    last_used_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_user_identities_provider_subject UNIQUE (provider, provider_subject),
    CONSTRAINT fk_user_identities_provider FOREIGN KEY (provider) REFERENCES iam.identity_providers(code),
    CONSTRAINT ck_user_identities_local_password CHECK (
        (provider = 'local' AND password_hash IS NOT NULL)
        OR (provider <> 'local' AND password_hash IS NULL)
    )
);

CREATE TABLE iam.user_sessions (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    user_id BIGINT NOT NULL REFERENCES iam.users(id) ON DELETE CASCADE,
    refresh_token_hash VARCHAR(500) NOT NULL,
    device_name VARCHAR(200) NULL,
    ip_address INET NULL,
    user_agent TEXT NULL,
    last_used_at TIMESTAMPTZ NULL,
    expires_at TIMESTAMPTZ NOT NULL,
    revoked_at TIMESTAMPTZ NULL,
    revoke_reason VARCHAR(200) NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_user_sessions_public_id UNIQUE (public_id),
    CONSTRAINT uq_user_sessions_refresh_token_hash UNIQUE (refresh_token_hash),
    CONSTRAINT ck_user_sessions_expiry CHECK (expires_at > created_at)
);

CREATE TABLE iam.email_verification_tokens (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id BIGINT NOT NULL REFERENCES iam.users(id) ON DELETE CASCADE,
    token_hash VARCHAR(500) NOT NULL,
    expires_at TIMESTAMPTZ NOT NULL,
    consumed_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_email_verification_tokens_hash UNIQUE (token_hash),
    CONSTRAINT ck_email_verification_tokens_expiry CHECK (expires_at > created_at)
);

CREATE TABLE iam.password_reset_tokens (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id BIGINT NOT NULL REFERENCES iam.users(id) ON DELETE CASCADE,
    token_hash VARCHAR(500) NOT NULL,
    expires_at TIMESTAMPTZ NOT NULL,
    consumed_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_password_reset_tokens_hash UNIQUE (token_hash),
    CONSTRAINT ck_password_reset_tokens_expiry CHECK (expires_at > created_at)
);

CREATE TABLE iam.user_roles (
    user_id BIGINT NOT NULL REFERENCES iam.users(id) ON DELETE CASCADE,
    role_id BIGINT NOT NULL REFERENCES iam.roles(id) ON DELETE RESTRICT,
    assigned_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    assigned_by_user_id BIGINT NULL REFERENCES iam.users(id),
    PRIMARY KEY (user_id, role_id)
);

CREATE TABLE iam.user_addresses (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id BIGINT NOT NULL REFERENCES iam.users(id) ON DELETE CASCADE,
    label VARCHAR(50) NULL,
    recipient_name VARCHAR(150) NULL,
    country_code CHAR(2) NULL,
    state_name VARCHAR(100) NULL,
    city_name VARCHAR(100) NULL,
    area_name VARCHAR(100) NULL,
    address_line_1 VARCHAR(200) NOT NULL,
    address_line_2 VARCHAR(200) NULL,
    postal_code VARCHAR(20) NULL,
    latitude NUMERIC(10,7) NULL,
    longitude NUMERIC(10,7) NULL,
    is_default BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT ck_user_addresses_latitude CHECK (latitude IS NULL OR latitude BETWEEN -90 AND 90),
    CONSTRAINT ck_user_addresses_longitude CHECK (longitude IS NULL OR longitude BETWEEN -180 AND 180)
);

/* =========================
   API-driven geography and catalog
   ========================= */

CREATE TABLE catalog.amenity_categories (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    code VARCHAR(50) NOT NULL,
    name VARCHAR(100) NOT NULL,
    CONSTRAINT uq_amenity_categories_code UNIQUE (code),
    CONSTRAINT uq_amenity_categories_name UNIQUE (name)
);

CREATE TABLE catalog.amenities (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    category_id BIGINT NULL REFERENCES catalog.amenity_categories(id) ON DELETE SET NULL,
    code VARCHAR(50) NOT NULL,
    name VARCHAR(100) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT uq_amenities_code UNIQUE (code),
    CONSTRAINT uq_amenities_name UNIQUE (name)
);

CREATE TABLE catalog.event_service_categories (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    code VARCHAR(50) NOT NULL,
    name VARCHAR(100) NOT NULL,
    CONSTRAINT uq_event_service_categories_code UNIQUE (code),
    CONSTRAINT uq_event_service_categories_name UNIQUE (name)
);

/* =========================
   Partner and property domain
   ========================= */

CREATE TABLE core.partners (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    business_name VARCHAR(200) NOT NULL,
    legal_name VARCHAR(200) NULL,
    email CITEXT NULL,
    phone VARCHAR(30) NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'pending',
    tax_identifier VARCHAR(100) NULL,
    billing_currency CHAR(3) NOT NULL DEFAULT 'USD',
    approved_by_user_id BIGINT NULL REFERENCES iam.users(id),
    approved_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_partners_public_id UNIQUE (public_id),
    CONSTRAINT fk_partners_status FOREIGN KEY (status) REFERENCES core.partner_statuses(code)
);

CREATE TABLE core.partner_users (
    partner_id BIGINT NOT NULL REFERENCES core.partners(id) ON DELETE CASCADE,
    user_id BIGINT NOT NULL REFERENCES iam.users(id) ON DELETE CASCADE,
    access_level VARCHAR(30) NOT NULL,
    joined_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    PRIMARY KEY (partner_id, user_id),
    CONSTRAINT fk_partner_users_access_level FOREIGN KEY (access_level) REFERENCES iam.partner_access_levels(code)
);

CREATE TABLE catalog.properties (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    partner_id BIGINT NOT NULL REFERENCES core.partners(id) ON DELETE RESTRICT,
    property_type VARCHAR(30) NOT NULL,
    slug VARCHAR(180) NOT NULL,
    name VARCHAR(200) NOT NULL,
    short_description VARCHAR(500) NULL,
    description TEXT NULL,
    country_code CHAR(2) NOT NULL,
    state_name VARCHAR(100) NULL,
    city_code VARCHAR(100) NULL,
    city_name VARCHAR(100) NOT NULL,
    area_code VARCHAR(100) NULL,
    area_name VARCHAR(100) NULL,
    address_line_1 VARCHAR(200) NOT NULL,
    address_line_2 VARCHAR(200) NULL,
    latitude NUMERIC(10,7) NULL,
    longitude NUMERIC(10,7) NULL,
    star_rating INT NULL,
    check_in_time TIME NULL,
    check_out_time TIME NULL,
    contact_email CITEXT NULL,
    contact_phone VARCHAR(30) NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'draft',
    is_featured BOOLEAN NOT NULL DEFAULT FALSE,
    published_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_properties_public_id UNIQUE (public_id),
    CONSTRAINT uq_properties_id_partner UNIQUE (id, partner_id),
    CONSTRAINT uq_properties_slug UNIQUE (slug),
    CONSTRAINT fk_properties_type FOREIGN KEY (property_type) REFERENCES catalog.property_types(code),
    CONSTRAINT fk_properties_status FOREIGN KEY (status) REFERENCES catalog.property_statuses(code),
    CONSTRAINT ck_properties_latitude CHECK (latitude IS NULL OR latitude BETWEEN -90 AND 90),
    CONSTRAINT ck_properties_longitude CHECK (longitude IS NULL OR longitude BETWEEN -180 AND 180),
    CONSTRAINT ck_properties_star_rating CHECK (star_rating IS NULL OR star_rating BETWEEN 1 AND 5)
);

CREATE TABLE catalog.property_images (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    property_id BIGINT NOT NULL REFERENCES catalog.properties(id) ON DELETE CASCADE,
    file_url TEXT NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    alt_text VARCHAR(200) NULL,
    is_cover BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_property_images_file_url UNIQUE (property_id, file_url)
);

CREATE TABLE catalog.property_policies (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    property_id BIGINT NOT NULL REFERENCES catalog.properties(id) ON DELETE CASCADE,
    policy_type VARCHAR(30) NOT NULL,
    title VARCHAR(150) NOT NULL,
    body TEXT NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_property_policies_type FOREIGN KEY (policy_type) REFERENCES catalog.policy_types(code)
);

CREATE TABLE catalog.property_amenities (
    property_id BIGINT NOT NULL REFERENCES catalog.properties(id) ON DELETE CASCADE,
    amenity_id BIGINT NOT NULL REFERENCES catalog.amenities(id) ON DELETE RESTRICT,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    PRIMARY KEY (property_id, amenity_id)
);

CREATE TABLE catalog.room_types (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    property_id BIGINT NOT NULL REFERENCES catalog.properties(id) ON DELETE CASCADE,
    slug VARCHAR(180) NOT NULL,
    name VARCHAR(150) NOT NULL,
    description TEXT NULL,
    max_adults INT NOT NULL,
    max_children INT NOT NULL DEFAULT 0,
    max_occupancy INT NOT NULL,
    bed_type VARCHAR(100) NULL,
    room_size_sqft NUMERIC(10,2) NULL,
    base_currency CHAR(3) NOT NULL,
    base_price NUMERIC(14,2) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_room_types_public_id UNIQUE (public_id),
    CONSTRAINT uq_room_types_slug UNIQUE (property_id, slug),
    CONSTRAINT ck_room_types_adults CHECK (max_adults >= 1),
    CONSTRAINT ck_room_types_children CHECK (max_children >= 0),
    CONSTRAINT ck_room_types_occupancy CHECK (max_occupancy >= max_adults),
    CONSTRAINT ck_room_types_base_price CHECK (base_price >= 0)
);

CREATE TABLE catalog.room_type_images (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    room_type_id BIGINT NOT NULL REFERENCES catalog.room_types(id) ON DELETE CASCADE,
    file_url TEXT NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_room_type_images_file_url UNIQUE (room_type_id, file_url)
);

CREATE TABLE catalog.packages (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    property_id BIGINT NOT NULL REFERENCES catalog.properties(id) ON DELETE CASCADE,
    room_type_id BIGINT NULL REFERENCES catalog.room_types(id) ON DELETE SET NULL,
    package_type VARCHAR(20) NOT NULL,
    slug VARCHAR(180) NOT NULL,
    title VARCHAR(200) NOT NULL,
    description TEXT NULL,
    inclusions TEXT NULL,
    exclusions TEXT NULL,
    max_guests INT NULL,
    duration_hours INT NULL,
    base_currency CHAR(3) NOT NULL,
    base_price NUMERIC(14,2) NOT NULL,
    valid_from DATE NULL,
    valid_until DATE NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_packages_public_id UNIQUE (public_id),
    CONSTRAINT uq_packages_slug UNIQUE (property_id, slug),
    CONSTRAINT fk_packages_type FOREIGN KEY (package_type) REFERENCES catalog.package_types(code),
    CONSTRAINT ck_packages_price CHECK (base_price >= 0),
    CONSTRAINT ck_packages_dates CHECK (valid_from IS NULL OR valid_until IS NULL OR valid_from <= valid_until)
);

CREATE TABLE catalog.activities (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    property_id BIGINT NOT NULL REFERENCES catalog.properties(id) ON DELETE CASCADE,
    slug VARCHAR(180) NOT NULL,
    title VARCHAR(200) NOT NULL,
    description TEXT NULL,
    restrictions TEXT NULL,
    duration_minutes INT NULL,
    base_currency CHAR(3) NOT NULL,
    base_price NUMERIC(14,2) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_activities_public_id UNIQUE (public_id),
    CONSTRAINT uq_activities_slug UNIQUE (property_id, slug),
    CONSTRAINT ck_activities_price CHECK (base_price >= 0)
);

CREATE TABLE catalog.event_venues (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    property_id BIGINT NOT NULL REFERENCES catalog.properties(id) ON DELETE CASCADE,
    slug VARCHAR(180) NOT NULL,
    name VARCHAR(200) NOT NULL,
    description TEXT NULL,
    indoor_outdoor VARCHAR(20) NOT NULL,
    min_capacity INT NULL,
    max_capacity INT NOT NULL,
    base_currency CHAR(3) NOT NULL,
    base_price NUMERIC(14,2) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_event_venues_public_id UNIQUE (public_id),
    CONSTRAINT uq_event_venues_slug UNIQUE (property_id, slug),
    CONSTRAINT fk_event_venues_placement FOREIGN KEY (indoor_outdoor) REFERENCES catalog.venue_placements(code),
    CONSTRAINT ck_event_venues_capacity CHECK (max_capacity > 0 AND (min_capacity IS NULL OR min_capacity <= max_capacity)),
    CONSTRAINT ck_event_venues_price CHECK (base_price >= 0)
);

CREATE TABLE catalog.transport_options (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    property_id BIGINT NOT NULL REFERENCES catalog.properties(id) ON DELETE CASCADE,
    title VARCHAR(150) NOT NULL,
    transport_type VARCHAR(30) NOT NULL,
    route_label VARCHAR(200) NULL,
    trip_type VARCHAR(20) NULL,
    max_passengers INT NULL,
    base_currency CHAR(3) NOT NULL,
    base_price NUMERIC(14,2) NOT NULL,
    requires_manual_confirmation BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_transport_options_public_id UNIQUE (public_id),
    CONSTRAINT fk_transport_options_type FOREIGN KEY (transport_type) REFERENCES catalog.transport_types(code),
    CONSTRAINT fk_transport_options_trip_type FOREIGN KEY (trip_type) REFERENCES catalog.trip_types(code),
    CONSTRAINT ck_transport_options_price CHECK (base_price >= 0)
);

CREATE TABLE catalog.service_addons (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    property_id BIGINT NOT NULL REFERENCES catalog.properties(id) ON DELETE CASCADE,
    category_id BIGINT NULL REFERENCES catalog.event_service_categories(id) ON DELETE SET NULL,
    name VARCHAR(150) NOT NULL,
    description TEXT NULL,
    pricing_model VARCHAR(30) NOT NULL,
    base_currency CHAR(3) NOT NULL,
    base_price NUMERIC(14,2) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_service_addons_public_id UNIQUE (public_id),
    CONSTRAINT fk_service_addons_pricing_model FOREIGN KEY (pricing_model) REFERENCES catalog.addon_pricing_models(code),
    CONSTRAINT ck_service_addons_price CHECK (base_price >= 0)
);

/* =========================
   Inventory and pricing
   ========================= */

CREATE TABLE inventory.room_inventory (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    room_type_id BIGINT NOT NULL REFERENCES catalog.room_types(id) ON DELETE CASCADE,
    inventory_date DATE NOT NULL,
    total_inventory INT NOT NULL,
    reserved_inventory INT NOT NULL DEFAULT 0,
    out_of_service_inventory INT NOT NULL DEFAULT 0,
    price_override NUMERIC(14,2) NULL,
    min_stay_nights INT NULL,
    max_stay_nights INT NULL,
    closed_to_arrival BOOLEAN NOT NULL DEFAULT FALSE,
    closed_to_departure BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_room_inventory UNIQUE (room_type_id, inventory_date),
    CONSTRAINT ck_room_inventory_total CHECK (total_inventory >= 0),
    CONSTRAINT ck_room_inventory_reserved CHECK (reserved_inventory >= 0 AND reserved_inventory <= total_inventory),
    CONSTRAINT ck_room_inventory_oos CHECK (out_of_service_inventory >= 0 AND out_of_service_inventory <= total_inventory),
    CONSTRAINT ck_room_inventory_available CHECK (reserved_inventory + out_of_service_inventory <= total_inventory),
    CONSTRAINT ck_room_inventory_price CHECK (price_override IS NULL OR price_override >= 0)
);

CREATE TABLE inventory.package_inventory (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    package_id BIGINT NOT NULL REFERENCES catalog.packages(id) ON DELETE CASCADE,
    inventory_date DATE NOT NULL,
    total_inventory INT NOT NULL,
    reserved_inventory INT NOT NULL DEFAULT 0,
    price_override NUMERIC(14,2) NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_package_inventory UNIQUE (package_id, inventory_date),
    CONSTRAINT ck_package_inventory_total CHECK (total_inventory >= 0),
    CONSTRAINT ck_package_inventory_reserved CHECK (reserved_inventory >= 0 AND reserved_inventory <= total_inventory),
    CONSTRAINT ck_package_inventory_price CHECK (price_override IS NULL OR price_override >= 0)
);

CREATE TABLE inventory.activity_slots (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    activity_id BIGINT NOT NULL REFERENCES catalog.activities(id) ON DELETE CASCADE,
    start_at TIMESTAMPTZ NOT NULL,
    end_at TIMESTAMPTZ NOT NULL,
    capacity INT NOT NULL,
    reserved_count INT NOT NULL DEFAULT 0,
    price_override NUMERIC(14,2) NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'open',
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_activity_slots_start UNIQUE (activity_id, start_at),
    CONSTRAINT fk_activity_slots_status FOREIGN KEY (status) REFERENCES inventory.slot_statuses(code),
    CONSTRAINT ck_activity_slots_time CHECK (start_at < end_at),
    CONSTRAINT ck_activity_slots_capacity CHECK (capacity >= 0 AND reserved_count >= 0 AND reserved_count <= capacity),
    CONSTRAINT ck_activity_slots_price CHECK (price_override IS NULL OR price_override >= 0)
);

CREATE TABLE inventory.venue_slots (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    venue_id BIGINT NOT NULL REFERENCES catalog.event_venues(id) ON DELETE CASCADE,
    start_at TIMESTAMPTZ NOT NULL,
    end_at TIMESTAMPTZ NOT NULL,
    capacity INT NOT NULL,
    price_override NUMERIC(14,2) NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'open',
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_venue_slots_start UNIQUE (venue_id, start_at),
    CONSTRAINT fk_venue_slots_status FOREIGN KEY (status) REFERENCES inventory.venue_slot_statuses(code),
    CONSTRAINT ck_venue_slots_time CHECK (start_at < end_at),
    CONSTRAINT ck_venue_slots_capacity CHECK (capacity > 0),
    CONSTRAINT ck_venue_slots_price CHECK (price_override IS NULL OR price_override >= 0)
);

/* =========================
   Promotions
   ========================= */

CREATE TABLE booking.coupons (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    code VARCHAR(50) NOT NULL,
    discount_type VARCHAR(20) NOT NULL,
    discount_value NUMERIC(14,2) NOT NULL,
    max_discount_amount NUMERIC(14,2) NULL,
    min_booking_amount NUMERIC(14,2) NULL,
    starts_at TIMESTAMPTZ NULL,
    ends_at TIMESTAMPTZ NULL,
    usage_limit_total INT NULL,
    usage_limit_per_user INT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_by_user_id BIGINT NULL REFERENCES iam.users(id),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_coupons_public_id UNIQUE (public_id),
    CONSTRAINT uq_coupons_code UNIQUE (code),
    CONSTRAINT fk_coupons_discount_type FOREIGN KEY (discount_type) REFERENCES booking.discount_types(code),
    CONSTRAINT ck_coupons_discount_value CHECK (discount_value >= 0),
    CONSTRAINT ck_coupons_max_discount CHECK (max_discount_amount IS NULL OR max_discount_amount >= 0),
    CONSTRAINT ck_coupons_min_booking CHECK (min_booking_amount IS NULL OR min_booking_amount >= 0),
    CONSTRAINT ck_coupons_dates CHECK (starts_at IS NULL OR ends_at IS NULL OR starts_at <= ends_at)
);

CREATE TABLE booking.bookings (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    booking_number VARCHAR(30) NOT NULL,
    customer_user_id BIGINT NOT NULL REFERENCES iam.users(id),
    property_id BIGINT NOT NULL,
    partner_id BIGINT NOT NULL,
    booking_type VARCHAR(20) NOT NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'draft',
    currency_code CHAR(3) NOT NULL,
    check_in_date DATE NULL,
    check_out_date DATE NULL,
    starts_at TIMESTAMPTZ NULL,
    ends_at TIMESTAMPTZ NULL,
    guest_adults INT NOT NULL DEFAULT 1,
    guest_children INT NOT NULL DEFAULT 0,
    subtotal_amount NUMERIC(14,2) NOT NULL DEFAULT 0,
    discount_amount NUMERIC(14,2) NOT NULL DEFAULT 0,
    tax_amount NUMERIC(14,2) NOT NULL DEFAULT 0,
    service_fee_amount NUMERIC(14,2) NOT NULL DEFAULT 0,
    total_amount NUMERIC(14,2) NOT NULL DEFAULT 0,
    amount_due NUMERIC(14,2) NOT NULL DEFAULT 0,
    coupon_id BIGINT NULL REFERENCES booking.coupons(id),
    source_channel VARCHAR(30) NOT NULL DEFAULT 'web',
    special_request TEXT NULL,
    cancellation_reason VARCHAR(500) NULL,
    cancelled_at TIMESTAMPTZ NULL,
    completed_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_bookings_property_partner FOREIGN KEY (property_id, partner_id)
        REFERENCES catalog.properties(id, partner_id),
    CONSTRAINT fk_bookings_type FOREIGN KEY (booking_type) REFERENCES booking.booking_types(code),
    CONSTRAINT fk_bookings_status FOREIGN KEY (status) REFERENCES booking.booking_statuses(code),
    CONSTRAINT uq_bookings_public_id UNIQUE (public_id),
    CONSTRAINT uq_bookings_id_customer UNIQUE (id, customer_user_id),
    CONSTRAINT uq_bookings_id_customer_property UNIQUE (id, customer_user_id, property_id),
    CONSTRAINT uq_bookings_number UNIQUE (booking_number),
    CONSTRAINT ck_bookings_amounts CHECK (
        subtotal_amount >= 0 AND discount_amount >= 0 AND tax_amount >= 0 AND
        service_fee_amount >= 0 AND total_amount >= 0 AND amount_due >= 0 AND amount_due <= total_amount
    ),
    CONSTRAINT ck_bookings_date_range CHECK (
        (check_in_date IS NULL OR check_out_date IS NULL OR check_in_date <= check_out_date) AND
        (starts_at IS NULL OR ends_at IS NULL OR starts_at < ends_at)
    )
);

CREATE TABLE booking.coupon_redemptions (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    coupon_id BIGINT NOT NULL REFERENCES booking.coupons(id) ON DELETE RESTRICT,
    booking_id BIGINT NOT NULL,
    user_id BIGINT NOT NULL REFERENCES iam.users(id) ON DELETE RESTRICT,
    discount_amount NUMERIC(14,2) NOT NULL,
    redeemed_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_coupon_redemptions_booking UNIQUE (booking_id),
    CONSTRAINT fk_coupon_redemptions_booking FOREIGN KEY (booking_id) REFERENCES booking.bookings(id) ON DELETE CASCADE,
    CONSTRAINT fk_coupon_redemptions_booking_user FOREIGN KEY (booking_id, user_id)
        REFERENCES booking.bookings(id, customer_user_id),
    CONSTRAINT ck_coupon_redemptions_amount CHECK (discount_amount >= 0)
);

CREATE TABLE booking.booking_status_history (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    booking_id BIGINT NOT NULL REFERENCES booking.bookings(id) ON DELETE CASCADE,
    from_status VARCHAR(30) NULL,
    to_status VARCHAR(30) NOT NULL,
    changed_by_user_id BIGINT NULL REFERENCES iam.users(id),
    reason VARCHAR(500) NULL,
    changed_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_booking_status_history_from_status FOREIGN KEY (from_status) REFERENCES booking.booking_statuses(code),
    CONSTRAINT fk_booking_status_history_to_status FOREIGN KEY (to_status) REFERENCES booking.booking_statuses(code)
);

CREATE TABLE booking.booking_guests (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    booking_id BIGINT NOT NULL REFERENCES booking.bookings(id) ON DELETE CASCADE,
    full_name VARCHAR(150) NOT NULL,
    email CITEXT NULL,
    phone VARCHAR(30) NULL,
    guest_type VARCHAR(20) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_booking_guests_type FOREIGN KEY (guest_type) REFERENCES booking.guest_types(code)
);

CREATE TABLE booking.booking_room_items (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    booking_id BIGINT NOT NULL REFERENCES booking.bookings(id) ON DELETE CASCADE,
    room_type_id BIGINT NOT NULL REFERENCES catalog.room_types(id) ON DELETE RESTRICT,
    room_inventory_date DATE NOT NULL,
    quantity INT NOT NULL,
    unit_price NUMERIC(14,2) NOT NULL,
    total_price NUMERIC(14,2) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT ck_booking_room_items_qty CHECK (quantity > 0),
    CONSTRAINT ck_booking_room_items_price CHECK (unit_price >= 0 AND total_price >= 0)
);

CREATE TABLE booking.booking_package_items (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    booking_id BIGINT NOT NULL REFERENCES booking.bookings(id) ON DELETE CASCADE,
    package_id BIGINT NOT NULL REFERENCES catalog.packages(id) ON DELETE RESTRICT,
    package_inventory_date DATE NOT NULL,
    quantity INT NOT NULL DEFAULT 1,
    unit_price NUMERIC(14,2) NOT NULL,
    total_price NUMERIC(14,2) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT ck_booking_package_items_qty CHECK (quantity > 0),
    CONSTRAINT ck_booking_package_items_price CHECK (unit_price >= 0 AND total_price >= 0)
);

CREATE TABLE booking.booking_activity_items (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    booking_id BIGINT NOT NULL REFERENCES booking.bookings(id) ON DELETE CASCADE,
    activity_id BIGINT NOT NULL REFERENCES catalog.activities(id) ON DELETE RESTRICT,
    activity_slot_id BIGINT NOT NULL REFERENCES inventory.activity_slots(id) ON DELETE RESTRICT,
    quantity INT NOT NULL DEFAULT 1,
    unit_price NUMERIC(14,2) NOT NULL,
    total_price NUMERIC(14,2) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT ck_booking_activity_items_qty CHECK (quantity > 0),
    CONSTRAINT ck_booking_activity_items_price CHECK (unit_price >= 0 AND total_price >= 0)
);

CREATE TABLE booking.booking_venue_items (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    booking_id BIGINT NOT NULL REFERENCES booking.bookings(id) ON DELETE CASCADE,
    venue_id BIGINT NOT NULL REFERENCES catalog.event_venues(id) ON DELETE RESTRICT,
    venue_slot_id BIGINT NOT NULL REFERENCES inventory.venue_slots(id) ON DELETE RESTRICT,
    guest_count INT NOT NULL,
    unit_price NUMERIC(14,2) NOT NULL,
    total_price NUMERIC(14,2) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT ck_booking_venue_items_guest_count CHECK (guest_count > 0),
    CONSTRAINT ck_booking_venue_items_price CHECK (unit_price >= 0 AND total_price >= 0)
);

CREATE TABLE booking.booking_transport_items (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    booking_id BIGINT NOT NULL REFERENCES booking.bookings(id) ON DELETE CASCADE,
    transport_option_id BIGINT NOT NULL REFERENCES catalog.transport_options(id) ON DELETE RESTRICT,
    quantity INT NOT NULL DEFAULT 1,
    pickup_at TIMESTAMPTZ NULL,
    dropoff_at TIMESTAMPTZ NULL,
    unit_price NUMERIC(14,2) NOT NULL,
    total_price NUMERIC(14,2) NOT NULL,
    details_json JSONB NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT ck_booking_transport_items_qty CHECK (quantity > 0),
    CONSTRAINT ck_booking_transport_items_price CHECK (unit_price >= 0 AND total_price >= 0)
);

CREATE TABLE booking.booking_addon_items (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    booking_id BIGINT NOT NULL REFERENCES booking.bookings(id) ON DELETE CASCADE,
    service_addon_id BIGINT NOT NULL REFERENCES catalog.service_addons(id) ON DELETE RESTRICT,
    quantity INT NOT NULL DEFAULT 1,
    service_at TIMESTAMPTZ NULL,
    unit_price NUMERIC(14,2) NOT NULL,
    total_price NUMERIC(14,2) NOT NULL,
    details_json JSONB NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT ck_booking_addon_items_qty CHECK (quantity > 0),
    CONSTRAINT ck_booking_addon_items_price CHECK (unit_price >= 0 AND total_price >= 0)
);

/* =========================
   Billing
   ========================= */

CREATE TABLE billing.payments (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    booking_id BIGINT NOT NULL REFERENCES booking.bookings(id) ON DELETE RESTRICT,
    provider VARCHAR(30) NOT NULL,
    payment_method_type VARCHAR(30) NOT NULL,
    provider_payment_intent_id VARCHAR(255) NULL,
    provider_charge_id VARCHAR(255) NULL,
    amount NUMERIC(14,2) NOT NULL,
    currency_code CHAR(3) NOT NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'pending',
    idempotency_key VARCHAR(100) NULL,
    client_secret VARCHAR(500) NULL,
    requires_action_at TIMESTAMPTZ NULL,
    paid_at TIMESTAMPTZ NULL,
    failed_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_payments_public_id UNIQUE (public_id),
    CONSTRAINT fk_payments_provider FOREIGN KEY (provider) REFERENCES billing.payment_providers(code),
    CONSTRAINT fk_payments_method FOREIGN KEY (payment_method_type) REFERENCES billing.payment_method_types(code),
    CONSTRAINT fk_payments_status FOREIGN KEY (status) REFERENCES billing.payment_statuses(code),
    CONSTRAINT ck_payments_provider_method CHECK (
        (payment_method_type IN ('card', 'wallet') AND provider = 'stripe')
        OR (payment_method_type = 'points' AND provider = 'internal')
        OR (payment_method_type = 'manual' AND provider = 'manual')
    ),
    CONSTRAINT ck_payments_amount CHECK (amount >= 0)
);

CREATE TABLE billing.payment_status_history (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    payment_id BIGINT NOT NULL REFERENCES billing.payments(id) ON DELETE CASCADE,
    from_status VARCHAR(30) NULL,
    to_status VARCHAR(30) NOT NULL,
    reason VARCHAR(500) NULL,
    changed_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_payment_status_history_from_status FOREIGN KEY (from_status) REFERENCES billing.payment_statuses(code),
    CONSTRAINT fk_payment_status_history_to_status FOREIGN KEY (to_status) REFERENCES billing.payment_statuses(code)
);

CREATE TABLE billing.refunds (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    payment_id BIGINT NOT NULL REFERENCES billing.payments(id) ON DELETE RESTRICT,
    provider_refund_id VARCHAR(255) NULL,
    amount NUMERIC(14,2) NOT NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'pending',
    reason VARCHAR(500) NULL,
    requested_by_user_id BIGINT NULL REFERENCES iam.users(id),
    requested_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    completed_at TIMESTAMPTZ NULL,
    CONSTRAINT uq_refunds_public_id UNIQUE (public_id),
    CONSTRAINT fk_refunds_status FOREIGN KEY (status) REFERENCES billing.refund_statuses(code),
    CONSTRAINT ck_refunds_amount CHECK (amount >= 0)
);

/* =========================
   External events and idempotency
   ========================= */

CREATE TABLE integration.webhook_events (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    provider VARCHAR(30) NOT NULL,
    event_id VARCHAR(255) NOT NULL,
    event_type VARCHAR(100) NOT NULL,
    payload JSONB NOT NULL,
    received_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    processed_at TIMESTAMPTZ NULL,
    processing_status VARCHAR(20) NOT NULL DEFAULT 'pending',
    related_payment_id BIGINT NULL REFERENCES billing.payments(id) ON DELETE SET NULL,
    error_message VARCHAR(1000) NULL,
    CONSTRAINT uq_webhook_events_provider_event UNIQUE (provider, event_id),
    CONSTRAINT fk_webhook_events_provider FOREIGN KEY (provider) REFERENCES billing.payment_providers(code),
    CONSTRAINT fk_webhook_events_processing_status FOREIGN KEY (processing_status) REFERENCES integration.webhook_processing_statuses(code)
);

/* =========================
   Reviews and engagement
   ========================= */

CREATE TABLE engagement.reviews (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    public_id UUID NOT NULL DEFAULT gen_random_uuid(),
    booking_id BIGINT NOT NULL,
    user_id BIGINT NOT NULL,
    property_id BIGINT NOT NULL,
    rating INT NOT NULL,
    title VARCHAR(150) NULL,
    body TEXT NULL,
    partner_response TEXT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'published',
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_reviews_booking_user_property FOREIGN KEY (booking_id, user_id, property_id)
        REFERENCES booking.bookings(id, customer_user_id, property_id),
    CONSTRAINT uq_reviews_public_id UNIQUE (public_id),
    CONSTRAINT uq_reviews_booking UNIQUE (booking_id),
    CONSTRAINT fk_reviews_status FOREIGN KEY (status) REFERENCES engagement.review_statuses(code),
    CONSTRAINT ck_reviews_rating CHECK (rating BETWEEN 1 AND 5)
);

/* =========================
   Helpful indexes
   ========================= */

CREATE UNIQUE INDEX ux_users_email_active
ON iam.users(email)
WHERE email IS NOT NULL AND deleted_at IS NULL;

CREATE UNIQUE INDEX ux_users_phone_active
ON iam.users(phone)
WHERE phone IS NOT NULL AND deleted_at IS NULL;

CREATE INDEX ix_users_status
ON iam.users(status);

CREATE UNIQUE INDEX ux_user_identities_user_provider
ON iam.user_identities(user_id, provider);

CREATE UNIQUE INDEX ux_user_identities_provider_email
ON iam.user_identities(provider, provider_email)
WHERE provider_email IS NOT NULL;

CREATE INDEX ix_user_sessions_user_id
ON iam.user_sessions(user_id);

CREATE INDEX ix_user_sessions_expires_at
ON iam.user_sessions(expires_at);

CREATE INDEX ix_user_sessions_revoked_at
ON iam.user_sessions(revoked_at);

CREATE UNIQUE INDEX ux_user_addresses_default
ON iam.user_addresses(user_id)
WHERE is_default = TRUE;

CREATE INDEX ix_email_verification_tokens_user_id
ON iam.email_verification_tokens(user_id, expires_at);

CREATE INDEX ix_password_reset_tokens_user_id
ON iam.password_reset_tokens(user_id, expires_at);

CREATE INDEX ix_partner_users_user_id
ON core.partner_users(user_id);

CREATE INDEX ix_properties_partner_status
ON catalog.properties(partner_id, status);

CREATE INDEX ix_properties_city_area_status
ON catalog.properties(country_code, city_name, area_name, status);

CREATE UNIQUE INDEX ux_property_images_cover
ON catalog.property_images(property_id)
WHERE is_cover = TRUE;

CREATE INDEX ix_property_images_property_sort
ON catalog.property_images(property_id, sort_order);

CREATE INDEX ix_room_type_images_room_type_sort
ON catalog.room_type_images(room_type_id, sort_order);

CREATE INDEX ix_room_inventory_lookup
ON inventory.room_inventory(room_type_id, inventory_date);

CREATE INDEX ix_package_inventory_lookup
ON inventory.package_inventory(package_id, inventory_date);

CREATE INDEX ix_activity_slots_lookup
ON inventory.activity_slots(activity_id, start_at, status);

CREATE INDEX ix_venue_slots_lookup
ON inventory.venue_slots(venue_id, start_at, status);

CREATE INDEX ix_bookings_customer_created
ON booking.bookings(customer_user_id, created_at DESC);

CREATE INDEX ix_bookings_property_status
ON booking.bookings(property_id, status);

CREATE INDEX ix_bookings_partner_status
ON booking.bookings(partner_id, status);

CREATE INDEX ix_booking_status_history_booking
ON booking.booking_status_history(booking_id, changed_at DESC);

CREATE INDEX ix_payments_booking_status
ON billing.payments(booking_id, status);

CREATE UNIQUE INDEX ux_payments_payment_intent
ON billing.payments(provider_payment_intent_id)
WHERE provider_payment_intent_id IS NOT NULL;

CREATE UNIQUE INDEX ux_payments_charge
ON billing.payments(provider_charge_id)
WHERE provider_charge_id IS NOT NULL;

CREATE UNIQUE INDEX ux_payments_idempotency_key
ON billing.payments(idempotency_key)
WHERE idempotency_key IS NOT NULL;

CREATE INDEX ix_payment_status_history_payment
ON billing.payment_status_history(payment_id, changed_at DESC);

CREATE INDEX ix_refunds_payment_status
ON billing.refunds(payment_id, status);

CREATE UNIQUE INDEX ux_refunds_provider_refund
ON billing.refunds(provider_refund_id)
WHERE provider_refund_id IS NOT NULL;

CREATE INDEX ix_webhook_events_status
ON integration.webhook_events(processing_status, received_at);

CREATE INDEX ix_reviews_property_status
ON engagement.reviews(property_id, status, created_at DESC);

/* =========================
   Updated-at triggers
   ========================= */

/* Trigger function */

CREATE OR REPLACE FUNCTION core.set_updated_at()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$;

/* Trigger installation */

DO $$
DECLARE
    target_table TEXT;
    tables TEXT[] := ARRAY[
        'iam.users',
        'iam.user_sessions',
        'iam.user_addresses',
        'core.partners',
        'catalog.properties',
        'catalog.property_policies',
        'catalog.room_types',
        'catalog.packages',
        'catalog.activities',
        'catalog.event_venues',
        'catalog.transport_options',
        'catalog.service_addons',
        'inventory.room_inventory',
        'inventory.package_inventory',
        'inventory.activity_slots',
        'inventory.venue_slots',
        'booking.bookings',
        'billing.payments',
        'engagement.reviews'
    ];
BEGIN
    FOREACH target_table IN ARRAY tables
    LOOP
        EXECUTE format('DROP TRIGGER IF EXISTS trg_set_updated_at ON %s', target_table);
        EXECUTE format(
            'CREATE TRIGGER trg_set_updated_at
             BEFORE UPDATE ON %s
             FOR EACH ROW
             EXECUTE FUNCTION core.set_updated_at()',
            target_table
        );
    END LOOP;
END $$;

/* =========================
   Status transition enforcement
   ========================= */

/* Trigger functions */

CREATE OR REPLACE FUNCTION booking.enforce_booking_status_transition()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    IF NEW.status IS NOT DISTINCT FROM OLD.status THEN
        RETURN NEW;
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM booking.booking_status_transitions t
        WHERE t.from_status = OLD.status
          AND t.to_status = NEW.status
          AND t.is_active = TRUE
    ) THEN
        RAISE EXCEPTION 'Invalid booking status transition: % -> %', OLD.status, NEW.status;
    END IF;

    RETURN NEW;
END;
$$;

CREATE OR REPLACE FUNCTION billing.enforce_payment_status_transition()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    IF NEW.status IS NOT DISTINCT FROM OLD.status THEN
        RETURN NEW;
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM billing.payment_status_transitions t
        WHERE t.from_status = OLD.status
          AND t.to_status = NEW.status
          AND t.is_active = TRUE
    ) THEN
        RAISE EXCEPTION 'Invalid payment status transition: % -> %', OLD.status, NEW.status;
    END IF;

    RETURN NEW;
END;
$$;

CREATE OR REPLACE FUNCTION billing.enforce_refund_status_transition()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    IF NEW.status IS NOT DISTINCT FROM OLD.status THEN
        RETURN NEW;
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM billing.refund_status_transitions t
        WHERE t.from_status = OLD.status
          AND t.to_status = NEW.status
          AND t.is_active = TRUE
    ) THEN
        RAISE EXCEPTION 'Invalid refund status transition: % -> %', OLD.status, NEW.status;
    END IF;

    RETURN NEW;
END;
$$;

/* Trigger installation */

DROP TRIGGER IF EXISTS trg_enforce_booking_status_transition ON booking.bookings;
CREATE TRIGGER trg_enforce_booking_status_transition
BEFORE UPDATE OF status ON booking.bookings
FOR EACH ROW
EXECUTE FUNCTION booking.enforce_booking_status_transition();

DROP TRIGGER IF EXISTS trg_enforce_payment_status_transition ON billing.payments;
CREATE TRIGGER trg_enforce_payment_status_transition
BEFORE UPDATE OF status ON billing.payments
FOR EACH ROW
EXECUTE FUNCTION billing.enforce_payment_status_transition();

DROP TRIGGER IF EXISTS trg_enforce_refund_status_transition ON billing.refunds;
CREATE TRIGGER trg_enforce_refund_status_transition
BEFORE UPDATE OF status ON billing.refunds
FOR EACH ROW
EXECUTE FUNCTION billing.enforce_refund_status_transition();

/* =========================
   Seed flexible reference data
   ========================= */

/* IAM seeds */

INSERT INTO iam.user_statuses (code, name, sort_order) VALUES
('active', 'Active', 1),
('pending_verification', 'Pending Verification', 2),
('suspended', 'Suspended', 3),
('deleted', 'Deleted', 4)
ON CONFLICT (code) DO NOTHING;

INSERT INTO iam.identity_providers (code, name, sort_order) VALUES
('local', 'Local', 1),
('google', 'Google', 2),
('phone_otp', 'Phone OTP', 3)
ON CONFLICT (code) DO NOTHING;

INSERT INTO iam.partner_access_levels (code, name, sort_order) VALUES
('owner', 'Owner', 1),
('manager', 'Manager', 2),
('finance', 'Finance', 3),
('operations', 'Operations', 4),
('viewer', 'Viewer', 5)
ON CONFLICT (code) DO NOTHING;

INSERT INTO core.partner_statuses (code, name, sort_order) VALUES
('pending', 'Pending', 1),
('active', 'Active', 2),
('suspended', 'Suspended', 3),
('rejected', 'Rejected', 4)
ON CONFLICT (code) DO NOTHING;

/* Catalog seeds */

INSERT INTO catalog.property_types (code, name, sort_order) VALUES
('hotel', 'Hotel', 1),
('resort', 'Resort', 2),
('villa', 'Villa', 3),
('lodge', 'Lodge', 4),
('apartment', 'Apartment', 5),
('event_resort', 'Event Resort', 6)
ON CONFLICT (code) DO NOTHING;

INSERT INTO catalog.property_statuses (code, name, sort_order) VALUES
('draft', 'Draft', 1),
('pending_review', 'Pending Review', 2),
('published', 'Published', 3),
('suspended', 'Suspended', 4),
('archived', 'Archived', 5)
ON CONFLICT (code) DO NOTHING;

INSERT INTO catalog.policy_types (code, name, sort_order) VALUES
('cancellation', 'Cancellation', 1),
('children', 'Children', 2),
('pets', 'Pets', 3),
('check_in', 'Check In', 4),
('payment', 'Payment', 5),
('house_rules', 'House Rules', 6),
('other', 'Other', 7)
ON CONFLICT (code) DO NOTHING;

INSERT INTO catalog.package_types (code, name, sort_order) VALUES
('staycation', 'Staycation', 1),
('daycation', 'Daycation', 2)
ON CONFLICT (code) DO NOTHING;

INSERT INTO catalog.venue_placements (code, name, sort_order) VALUES
('indoor', 'Indoor', 1),
('outdoor', 'Outdoor', 2),
('mixed', 'Mixed', 3)
ON CONFLICT (code) DO NOTHING;

INSERT INTO catalog.transport_types (code, name, sort_order) VALUES
('airport_pickup', 'Airport Pickup', 1),
('airport_drop', 'Airport Drop', 2),
('road_transfer', 'Road Transfer', 3),
('air_transfer_inquiry', 'Air Transfer Inquiry', 4),
('shuttle', 'Shuttle', 5)
ON CONFLICT (code) DO NOTHING;

INSERT INTO catalog.trip_types (code, name, sort_order) VALUES
('one_way', 'One Way', 1),
('round_trip', 'Round Trip', 2)
ON CONFLICT (code) DO NOTHING;

INSERT INTO catalog.addon_pricing_models (code, name, sort_order) VALUES
('fixed', 'Fixed', 1),
('per_guest', 'Per Guest', 2),
('per_hour', 'Per Hour', 3),
('per_unit', 'Per Unit', 4)
ON CONFLICT (code) DO NOTHING;

INSERT INTO inventory.slot_statuses (code, name, sort_order) VALUES
('open', 'Open', 1),
('closed', 'Closed', 2),
('cancelled', 'Cancelled', 3)
ON CONFLICT (code) DO NOTHING;

INSERT INTO inventory.venue_slot_statuses (code, name, sort_order) VALUES
('open', 'Open', 1),
('closed', 'Closed', 2),
('blocked', 'Blocked', 3),
('cancelled', 'Cancelled', 4)
ON CONFLICT (code) DO NOTHING;

/* Booking seeds */

INSERT INTO booking.discount_types (code, name, sort_order) VALUES
('percentage', 'Percentage', 1),
('fixed', 'Fixed', 2)
ON CONFLICT (code) DO NOTHING;

INSERT INTO booking.booking_types (code, name, sort_order) VALUES
('room', 'Room', 1),
('staycation', 'Staycation', 2),
('daycation', 'Daycation', 3),
('activity', 'Activity', 4),
('event', 'Event', 5)
ON CONFLICT (code) DO NOTHING;

INSERT INTO booking.booking_statuses (code, name, sort_order) VALUES
('draft', 'Draft', 1),
('pending_payment', 'Pending Payment', 2),
('confirmed', 'Confirmed', 3),
('partially_paid', 'Partially Paid', 4),
('cancelled', 'Cancelled', 5),
('completed', 'Completed', 6),
('refund_pending', 'Refund Pending', 7),
('refunded', 'Refunded', 8),
('expired', 'Expired', 9)
ON CONFLICT (code) DO NOTHING;

INSERT INTO booking.booking_status_transitions (from_status, to_status) VALUES
('draft', 'pending_payment'),
('draft', 'expired'),
('pending_payment', 'confirmed'),
('pending_payment', 'partially_paid'),
('pending_payment', 'cancelled'),
('pending_payment', 'expired'),
('partially_paid', 'confirmed'),
('partially_paid', 'cancelled'),
('partially_paid', 'refund_pending'),
('confirmed', 'completed'),
('confirmed', 'cancelled'),
('confirmed', 'refund_pending'),
('refund_pending', 'refunded'),
('cancelled', 'refund_pending')
ON CONFLICT (from_status, to_status) DO NOTHING;

INSERT INTO booking.guest_types (code, name, sort_order) VALUES
('primary', 'Primary', 1),
('adult', 'Adult', 2),
('child', 'Child', 3)
ON CONFLICT (code) DO NOTHING;

INSERT INTO billing.payment_providers (code, name, sort_order) VALUES
('stripe', 'Stripe', 1),
('internal', 'Internal', 2),
('manual', 'Manual', 3)
ON CONFLICT (code) DO NOTHING;

INSERT INTO billing.payment_method_types (code, name, sort_order) VALUES
('card', 'Card', 1),
('wallet', 'Wallet', 2),
('points', 'Points', 3),
('manual', 'Manual', 4)
ON CONFLICT (code) DO NOTHING;

INSERT INTO billing.payment_statuses (code, name, sort_order) VALUES
('pending', 'Pending', 1),
('requires_action', 'Requires Action', 2),
('authorized', 'Authorized', 3),
('paid', 'Paid', 4),
('failed', 'Failed', 5),
('cancelled', 'Cancelled', 6),
('refund_pending', 'Refund Pending', 7),
('refunded', 'Refunded', 8),
('partially_refunded', 'Partially Refunded', 9)
ON CONFLICT (code) DO NOTHING;

INSERT INTO billing.payment_status_transitions (from_status, to_status) VALUES
('pending', 'requires_action'),
('pending', 'authorized'),
('pending', 'paid'),
('pending', 'failed'),
('pending', 'cancelled'),
('requires_action', 'paid'),
('requires_action', 'failed'),
('authorized', 'paid'),
('authorized', 'cancelled'),
('paid', 'refund_pending'),
('refund_pending', 'refunded'),
('refund_pending', 'partially_refunded'),
('partially_refunded', 'refunded')
ON CONFLICT (from_status, to_status) DO NOTHING;

INSERT INTO billing.refund_statuses (code, name, sort_order) VALUES
('pending', 'Pending', 1),
('succeeded', 'Succeeded', 2),
('failed', 'Failed', 3),
('cancelled', 'Cancelled', 4)
ON CONFLICT (code) DO NOTHING;

INSERT INTO billing.refund_status_transitions (from_status, to_status) VALUES
('pending', 'succeeded'),
('pending', 'failed'),
('pending', 'cancelled')
ON CONFLICT (from_status, to_status) DO NOTHING;

/* Integration and engagement seeds */

INSERT INTO integration.webhook_processing_statuses (code, name, sort_order) VALUES
('pending', 'Pending', 1),
('processed', 'Processed', 2),
('failed', 'Failed', 3),
('ignored', 'Ignored', 4)
ON CONFLICT (code) DO NOTHING;

INSERT INTO engagement.review_statuses (code, name, sort_order) VALUES
('published', 'Published', 1),
('hidden', 'Hidden', 2),
('flagged', 'Flagged', 3)
ON CONFLICT (code) DO NOTHING;

/* =========================
   Seed roles
   ========================= */

INSERT INTO iam.roles (code, name)
VALUES
    ('customer', 'Customer'),
    ('partner', 'Partner'),
    ('admin', 'Admin'),
    ('corporate_manager', 'Corporate Manager')
ON CONFLICT (code) DO NOTHING;
