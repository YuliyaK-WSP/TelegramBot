-- Table: public.requests

-- DROP TABLE IF EXISTS public.requests;

CREATE TABLE IF NOT EXISTS public.requests
(
    "number" integer NOT NULL DEFAULT nextval('requests_number_seq'::regclass),
    specialist specialist,
    description text COLLATE pg_catalog."default",
    urgency character varying(255) COLLATE pg_catalog."default",
    status character varying(255) COLLATE pg_catalog."default",
    assignee character varying(255) COLLATE pg_catalog."default",
    creation_date timestamp without time zone,
    completion_date timestamp without time zone,
    work_rating integer,
    user_id integer,
    specialist_id integer,
    CONSTRAINT requests_pkey PRIMARY KEY ("number"),
    CONSTRAINT fk_specialist_id FOREIGN KEY (specialist_id)
        REFERENCES public.users (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT requests_user_id_fkey FOREIGN KEY (user_id)
        REFERENCES public.users (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.requests
    OWNER to postgres;
-- Table: public.users

-- DROP TABLE IF EXISTS public.users;

CREATE TABLE IF NOT EXISTS public.users
(
    id integer NOT NULL DEFAULT nextval('users_id_seq'::regclass),
    active_regist boolean,
    chat_id bigint,
    user_role user_roles,
    first_name character varying(255) COLLATE pg_catalog."default",
    last_name character varying(255) COLLATE pg_catalog."default",
    patronymic character varying(255) COLLATE pg_catalog."default",
    phone_number character varying(255) COLLATE pg_catalog."default",
    city character varying(255) COLLATE pg_catalog."default",
    street character varying(255) COLLATE pg_catalog."default",
    house_number character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT users_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.users
    OWNER to postgres;