-- 1. Ensure the 'subscriptions' table has an expiry date column
-- Note: 'current_period_end' is the standard column for expiry/renewal date.
-- If you specifically want a column named 'expiry_date', you can add it, 
-- but using 'current_period_end' is recommended and what the app currently uses.

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'subscriptions' AND column_name = 'current_period_end') THEN
        ALTER TABLE public.subscriptions ADD COLUMN current_period_end timestamp with time zone;
    END IF;
END $$;

-- 2. SQL to add a test subscription for a specific user (Replace 'USER_UUID_HERE' with the actual User ID)
-- You can find your User ID in the Supabase Authentication dashboard.
-- Example: 'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11'

/*
INSERT INTO public.subscriptions (user_id, status, plan_id, current_period_end)
VALUES (
  'USER_UUID_HERE',  -- Replace this with the target user's UUID
  'active',
  'test_plan',
  NOW() + INTERVAL '1 month' -- Expires in 1 month
)
ON CONFLICT (user_id) 
DO UPDATE SET 
  status = 'active',
  plan_id = 'test_plan',
  current_period_end = NOW() + INTERVAL '1 month';
*/

-- 3. SQL to add a test subscription for the *currently logged in* user (if running in SQL Editor context with user assumed)
-- Note: This usually only works if you are running it from an authenticated client or mimicking one. 
-- In the Supabase SQL Editor, you usually act as postgres/admin, so you must specify the UUID explicitly as above.

-- 4. Create a Helper Function to easily add a test subscription by Email
-- Check if function exists first to avoid errors on repeated runs
CREATE OR REPLACE FUNCTION public.add_test_subscription(user_email TEXT, duration_months INT DEFAULT 1)
RETURNS VOID AS $$
DECLARE
  target_user_id UUID;
BEGIN
  -- Find user ID by email
  SELECT id INTO target_user_id FROM auth.users WHERE email = user_email;
  
  IF target_user_id IS NULL THEN
    RAISE EXCEPTION 'User not found with email: %', user_email;
  END IF;

  -- Insert or Update Subscription
  INSERT INTO public.subscriptions (user_id, status, plan_id, current_period_end)
  VALUES (
    target_user_id,
    'active',
    'test_plan_v1',
    NOW() + (duration_months || ' months')::INTERVAL
  )
  ON CONFLICT (user_id) 
  DO UPDATE SET 
    status = 'active',
    plan_id = 'test_plan_v1',
    current_period_end = EXCLUDED.current_period_end;
    
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Usage Example:
-- SELECT public.add_test_subscription('alex@example.com');
