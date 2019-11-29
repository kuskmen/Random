﻿drop database if exists shameful_mvc
create database shameful_mvc

use shameful_mvc

drop table if exists students
create table students (
    facultyNumber varchar(10) primary key not null,
    name varchar(20) not null,
    middleName varchar(100),
    lastName varchar(100) not null,
    password varchar(30) not null,
    specialty varchar(100) not null,
    year tinyint not null,
    formOfEducation varchar(10) not null
);

drop table if exists assignments
create table assignments(
    date datetime not null,
    name varchar(100) not null,
    assignment varbinary not null,
);