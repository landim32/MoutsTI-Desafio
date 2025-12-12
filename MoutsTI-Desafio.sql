
CREATE TABLE public.employee_phones (
    phone_id bigint NOT NULL,
    employee_id bigint NOT NULL,
    phone character varying(25) NOT NULL
);


ALTER TABLE public.employee_phones OWNER TO moutsti_user;

--
-- TOC entry 219 (class 1259 OID 16418)
-- Name: employee_phones_phone_id_seq; Type: SEQUENCE; Schema: public; Owner: moutsti_user
--

CREATE SEQUENCE public.employee_phones_phone_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.employee_phones_phone_id_seq OWNER TO moutsti_user;

--
-- TOC entry 3444 (class 0 OID 0)
-- Dependencies: 219
-- Name: employee_phones_phone_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: moutsti_user
--

ALTER SEQUENCE public.employee_phones_phone_id_seq OWNED BY public.employee_phones.phone_id;


--
-- TOC entry 216 (class 1259 OID 16390)
-- Name: employee_roles; Type: TABLE; Schema: public; Owner: moutsti_user
--

CREATE TABLE public.employee_roles (
    role_id bigint NOT NULL,
    name character varying(80) NOT NULL,
    level integer DEFAULT 1 NOT NULL
);


ALTER TABLE public.employee_roles OWNER TO moutsti_user;

--
-- TOC entry 215 (class 1259 OID 16389)
-- Name: employee_roles_role_id_seq; Type: SEQUENCE; Schema: public; Owner: moutsti_user
--

CREATE SEQUENCE public.employee_roles_role_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.employee_roles_role_id_seq OWNER TO moutsti_user;

--
-- TOC entry 3445 (class 0 OID 0)
-- Dependencies: 215
-- Name: employee_roles_role_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: moutsti_user
--

ALTER SEQUENCE public.employee_roles_role_id_seq OWNED BY public.employee_roles.role_id;


--
-- TOC entry 218 (class 1259 OID 16398)
-- Name: employees; Type: TABLE; Schema: public; Owner: moutsti_user
--

CREATE TABLE public.employees (
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


ALTER TABLE public.employees OWNER TO moutsti_user;

--
-- TOC entry 217 (class 1259 OID 16397)
-- Name: employees_employee_id_seq; Type: SEQUENCE; Schema: public; Owner: moutsti_user
--

CREATE SEQUENCE public.employees_employee_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.employees_employee_id_seq OWNER TO moutsti_user;

--
-- TOC entry 3446 (class 0 OID 0)
-- Dependencies: 217
-- Name: employees_employee_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: moutsti_user
--

ALTER SEQUENCE public.employees_employee_id_seq OWNED BY public.employees.employee_id;


--
-- TOC entry 3277 (class 2604 OID 16422)
-- Name: employee_phones phone_id; Type: DEFAULT; Schema: public; Owner: moutsti_user
--

ALTER TABLE ONLY public.employee_phones ALTER COLUMN phone_id SET DEFAULT nextval('public.employee_phones_phone_id_seq'::regclass);


--
-- TOC entry 3274 (class 2604 OID 16393)
-- Name: employee_roles role_id; Type: DEFAULT; Schema: public; Owner: moutsti_user
--

ALTER TABLE ONLY public.employee_roles ALTER COLUMN role_id SET DEFAULT nextval('public.employee_roles_role_id_seq'::regclass);


--
-- TOC entry 3276 (class 2604 OID 16401)
-- Name: employees employee_id; Type: DEFAULT; Schema: public; Owner: moutsti_user
--

ALTER TABLE ONLY public.employees ALTER COLUMN employee_id SET DEFAULT nextval('public.employees_employee_id_seq'::regclass);


--
-- TOC entry 3437 (class 0 OID 16419)
-- Dependencies: 220
-- Data for Name: employee_phones; Type: TABLE DATA; Schema: public; Owner: moutsti_user
--

INSERT INTO public.employee_phones (phone_id, employee_id, phone) VALUES (42, 3, '61998752588');
INSERT INTO public.employee_phones (phone_id, employee_id, phone) VALUES (74, 2, '61998752588');


--
-- TOC entry 3433 (class 0 OID 16390)
-- Dependencies: 216
-- Data for Name: employee_roles; Type: TABLE DATA; Schema: public; Owner: moutsti_user
--

INSERT INTO public.employee_roles (role_id, name, level) VALUES (1, 'Employee', 3);
INSERT INTO public.employee_roles (role_id, name, level) VALUES (2, 'Leader', 2);
INSERT INTO public.employee_roles (role_id, name, level) VALUES (3, 'Director', 1);


--
-- TOC entry 3435 (class 0 OID 16398)
-- Dependencies: 218
-- Data for Name: employees; Type: TABLE DATA; Schema: public; Owner: moutsti_user
--

INSERT INTO public.employees (employee_id, role_id, manager_id, first_name, last_name, email, doc_number, password, birthday) VALUES (3, 1, 2, 'Viviane', 'Carneiro', 'vivi@gmail.com', '896.397.661-00', '', '1981-07-15 00:00:00+00');
INSERT INTO public.employees (employee_id, role_id, manager_id, first_name, last_name, email, doc_number, password, birthday) VALUES (2, 1, NULL, 'Rodrigo2', 'Carneiro', 'rodrigo@emagine.com.br', '89639766100', 'KJFg2w2fOfmuF1TE7JwW+QtQ4y4JxftUga5kKz09GjY=', '1978-10-26 00:00:00+00');


--
-- TOC entry 3447 (class 0 OID 0)
-- Dependencies: 219
-- Name: employee_phones_phone_id_seq; Type: SEQUENCE SET; Schema: public; Owner: moutsti_user
--

SELECT pg_catalog.setval('public.employee_phones_phone_id_seq', 74, true);


--
-- TOC entry 3448 (class 0 OID 0)
-- Dependencies: 215
-- Name: employee_roles_role_id_seq; Type: SEQUENCE SET; Schema: public; Owner: moutsti_user
--

SELECT pg_catalog.setval('public.employee_roles_role_id_seq', 3, true);


--
-- TOC entry 3449 (class 0 OID 0)
-- Dependencies: 217
-- Name: employees_employee_id_seq; Type: SEQUENCE SET; Schema: public; Owner: moutsti_user
--

SELECT pg_catalog.setval('public.employees_employee_id_seq', 35, true);


--
-- TOC entry 3285 (class 2606 OID 16424)
-- Name: employee_phones employee_phones_pkey; Type: CONSTRAINT; Schema: public; Owner: moutsti_user
--

ALTER TABLE ONLY public.employee_phones
    ADD CONSTRAINT employee_phones_pkey PRIMARY KEY (phone_id);


--
-- TOC entry 3279 (class 2606 OID 16396)
-- Name: employee_roles employee_roles_pkey; Type: CONSTRAINT; Schema: public; Owner: moutsti_user
--

ALTER TABLE ONLY public.employee_roles
    ADD CONSTRAINT employee_roles_pkey PRIMARY KEY (role_id);


--
-- TOC entry 3281 (class 2606 OID 16407)
-- Name: employees employees_doc_number_key; Type: CONSTRAINT; Schema: public; Owner: moutsti_user
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_doc_number_key UNIQUE (doc_number);


--
-- TOC entry 3283 (class 2606 OID 16405)
-- Name: employees employees_pkey; Type: CONSTRAINT; Schema: public; Owner: moutsti_user
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (employee_id);


--
-- TOC entry 3288 (class 2606 OID 16425)
-- Name: employee_phones employee_phones_employee_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: moutsti_user
--

ALTER TABLE ONLY public.employee_phones
    ADD CONSTRAINT employee_phones_employee_id_fkey FOREIGN KEY (employee_id) REFERENCES public.employees(employee_id) ON DELETE CASCADE;


--
-- TOC entry 3286 (class 2606 OID 16413)
-- Name: employees employees_manager_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: moutsti_user
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_manager_id_fkey FOREIGN KEY (manager_id) REFERENCES public.employees(employee_id) NOT VALID;


--
-- TOC entry 3287 (class 2606 OID 16408)
-- Name: employees employees_role_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: moutsti_user
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_role_id_fkey FOREIGN KEY (role_id) REFERENCES public.employee_roles(role_id);