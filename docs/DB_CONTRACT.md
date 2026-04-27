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
