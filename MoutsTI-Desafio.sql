
CREATE TABLE employee_phones (
    phone_id bigint NOT NULL,
    employee_id bigint NOT NULL,
    phone character varying(25) NOT NULL
);

CREATE TABLE employee_roles (
    role_id bigint NOT NULL,
    name character varying(80) NOT NULL,
    level integer DEFAULT 1 NOT NULL
);

CREATE TABLE employees (
    employee_id bigint NOT NULL,
    role_id bigint NOT NULL,
    manager_id bigint,
    first_name character varying(120) NOT NULL,
    last_name character varying(120) NOT NULL,
    email character varying(180) NOT NULL,
    doc_number character varying(25) NOT NULL,
    password character varying(520) NOT NULL,
    birthday timestamp with time zone NOT NULL
);

CREATE SEQUENCE employees_employee_id_seq
    START WITH 2
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE SEQUENCE employee_phones_phone_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE SEQUENCE employee_roles_role_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER TABLE ONLY employee_phones ALTER COLUMN phone_id SET DEFAULT nextval('public.employee_phones_phone_id_seq'::regclass);
ALTER TABLE ONLY employee_roles ALTER COLUMN role_id SET DEFAULT nextval('public.employee_roles_role_id_seq'::regclass);
ALTER TABLE ONLY employees ALTER COLUMN employee_id SET DEFAULT nextval('public.employees_employee_id_seq'::regclass);

INSERT INTO employee_roles (role_id, name, level) VALUES (1, 'Employee', 1);
INSERT INTO employee_roles (role_id, name, level) VALUES (2, 'Leader', 2);
INSERT INTO employee_roles (role_id, name, level) VALUES (3, 'Director', 3);

INSERT INTO employees (employee_id, role_id, manager_id, first_name, last_name, email, doc_number, password, birthday) 
     VALUES (
        1, 3, NULL, 
        'Rodrigo', 
        'Carneiro', 
        'rodrigo@emagine.com.br', 
        '89639766100', 
        'KJFg2w2fOfmuF1TE7JwW+QtQ4y4JxftUga5kKz09GjY=', 
        '1978-10-26 00:00:00+00'
     );

ALTER TABLE ONLY employee_phones
    ADD CONSTRAINT employee_phones_pkey PRIMARY KEY (phone_id);

ALTER TABLE ONLY employee_roles
    ADD CONSTRAINT employee_roles_pkey PRIMARY KEY (role_id);

ALTER TABLE ONLY employees
    ADD CONSTRAINT employees_doc_number_key UNIQUE (doc_number);

ALTER TABLE ONLY employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (employee_id);

ALTER TABLE ONLY employee_phones
    ADD CONSTRAINT employee_phones_employee_id_fkey FOREIGN KEY (employee_id) REFERENCES public.employees(employee_id) ON DELETE CASCADE;

ALTER TABLE ONLY employees
    ADD CONSTRAINT employees_manager_id_fkey FOREIGN KEY (manager_id) REFERENCES public.employees(employee_id) NOT VALID;

ALTER TABLE ONLY employees
    ADD CONSTRAINT employees_role_id_fkey FOREIGN KEY (role_id) REFERENCES public.employee_roles(role_id);