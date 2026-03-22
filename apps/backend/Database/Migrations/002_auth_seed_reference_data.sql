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
