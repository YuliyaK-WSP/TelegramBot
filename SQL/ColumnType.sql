-- Type: specialist

-- DROP TYPE IF EXISTS public.specialist;

CREATE TYPE public.specialist AS ENUM
    ('Plumber', 'Electrician');

ALTER TYPE public.specialist
    OWNER TO postgres;
-- Type: user_roles

-- DROP TYPE IF EXISTS public.user_roles;

CREATE TYPE public.user_roles AS ENUM
    ('admin', 'executor', 'default');

ALTER TYPE public.user_roles
    OWNER TO postgres;
