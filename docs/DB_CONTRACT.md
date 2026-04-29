# ChMS - Database Contract

This markdown lists all core database tables and their columns along with its types and constraints for the Church Management System (ChMS).

## 1. users (Authentication)

| Column        | Type      | Notes / Constraints       |
| ------------- | --------- | ------------------------- |
| id            | UUID      | Primary Key               |
| name          | String    | User's full name          |
| email         | String    | Unique, indexed           |
| password      | String    | Hashed                    |
| has_onboarded | Boolean   | Default: false            |
| role          | Enum      | member, admin, superadmin |
| is_active     | Boolean   | Default: true             |
| created_at    | Timestamp |                           |
| updated_at    | Timestamp |                           |

## 2. families (Central Unit for Families)

| Column      | Type      | Notes / Constraints                     |
| ----------- | --------- | --------------------------------------- |
| id          | UUID      | Primary Key                             |
| user_id     | UUID      | Foreign Key -> users                    |
| home_phone  | String    | Primary contact number                  |
| address     | Text      | Full address (street, city, state, pin) |
| annual_dues | Decimal   | e.g., 1200.00 (nullable)                |
| status      | Enum      | active, inactive, moved                 |
| notes       | Text      | Admin note (only visible to admin)      |
| created_at  | Timestamp |                                         |
| updated_at  | Timestamp |                                         |

## 3. family_members (Links members to families)

| Column          | Type      | Notes / Constraints      |
| --------------- | --------- | ------------------------ |
| id              | UUID      | Primary Key              |
| user_id         | UUID      | Foreign Key -> users     |
| family_id       | UUID      | Foreign Key -> families  |
| name            | String    | User's full name         |
| email           | String    | Unique, indexed          |
| primary_phone   | String    | Primary contact number   |
| secondary_phone | String    | Primary contact number   |
| gender          | Enum      | male, female             |
| dob             | Date      | Date of Birth            |
| is_primary      | Boolean   | True for primary contact |
| created_at      | Timestamp |                          |
| updated_at      | Timestamp |                          |

## 4. family_relationship (Links members to members)

| Column            | Type      | Notes / Constraints                      |
| ----------------- | --------- | ---------------------------------------- |
| id                | UUID      | PK                                       |
| family_id         | UUID      | FK -> families                           |
| member_id         | UUID      | FK -> family_members                     |
| related_member_id | UUID      | FK -> family_members                     |
| relationship_type | Enum      | spouse, parent, child, sibling, guardian |
| start_date        | Date      | marriage date, guardianship start        |
| end_date          | Date      | divorce, ended relation                  |
| is_active         | Boolean   | current relationship                     |
| created_at        | Timestamp |                                          |
| updated_at        | Timestamp |                                          |
