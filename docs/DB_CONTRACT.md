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
