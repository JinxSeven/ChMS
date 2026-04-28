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
| user_id     | UUID      | Foreign Key to users                    |
| family_name | String    | e.g., "Livingston"                      |
| home_phone  | String    | Primary contact number                  |
| address     | Text      | Full address (street, city, state, pin) |
| annual_dues | Decimal   | e.g., 1200.00 (nullable)                |
| status      | Enum      | active, inactive, moved                 |
| notes       | Text      | Admin note (only visible to admin)      |
| created_at  | Timestamp |                                         |
| updated_at  | Timestamp |                                         |

## 3. family_members (Links members to families)

| Column         | Type      | Notes / Constraints        |
| -------------- | --------- | -------------------------- |
| id             | UUID      | Primary Key                |
| user_id        | UUID      | Foreign Key to users       |
| family_id      | UUID      | Foreign Key to families    |
| name           | String    | User's full name           |
| email          | String    | Unique, indexed            |
| phone          | String    | Primary contact number     |
| gender         | Enum      | male, female               |
| dob            | Date      | Date of Birth              |
| relationship   | Enum      | head, spouse, child, other |
| marital_status | Enum      | single, married            |
| married_to     | UUID      | Id of person married to    |
| wedding_day    | UUID      | Id of person married to    |
| is_primary     | Boolean   | True for primary contact   |
| created_at     | Timestamp |                            |
| updated_at     | Timestamp |                            |
