CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

CREATE SCHEMA IF NOT EXISTS iam;

CREATE TABLE IF NOT EXISTS iam.user_statuses (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS iam.identity_providers (
    code VARCHAR(30) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS iam.users (
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

CREATE TABLE IF NOT EXISTS iam.user_identities (
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

CREATE TABLE IF NOT EXISTS iam.user_sessions (
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

CREATE TABLE IF NOT EXISTS iam.password_reset_tokens (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id BIGINT NOT NULL REFERENCES iam.users(id) ON DELETE CASCADE,
    token_hash VARCHAR(500) NOT NULL,
    expires_at TIMESTAMPTZ NOT NULL,
    consumed_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_password_reset_tokens_hash UNIQUE (token_hash),
    CONSTRAINT ck_password_reset_tokens_expiry CHECK (expires_at > created_at)
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_users_email_active
ON iam.users (email)
WHERE email IS NOT NULL AND deleted_at IS NULL;

CREATE UNIQUE INDEX IF NOT EXISTS ux_users_phone_active
ON iam.users (phone)
WHERE phone IS NOT NULL AND deleted_at IS NULL;
