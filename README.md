# TournamentManager-Portfolio
*A curated portfolio edition of an ASP.NET Core MVC tournament management system.*

## Overview
**Tournament Manager** is a web-based system for creating, managing, and participating in competitive tournaments. It supports multiple user roles, automated bracket generation, ELO-based player rankings, and public leaderboards.

This edition is a **portfolio reconstruction** of a collaborative student project, rebuilt to showcase my technical work in a clean, self-contained format.

## Purpose of This Edition
This repository contains a **fresh, single-commit import** of the project’s codebase.  
It is intentionally presented without prior commit history to:

- Protect collaborator privacy  
- Provide a clean, recruiter-friendly view of the architecture and implementation  
- Highlight my contributions to the system’s design and development  

# My Contributions

This portfolio edition highlights the portions of the project that I personally designed and implemented. My work focused heavily on the **backend architecture**, **tournament lifecycle**, **bracket generation**, and **ELO rating logic**—the core systems that make the application function as a real tournament platform.

## Core Areas of Ownership

### Tournament Management (Backend)
I implemented the majority of the Organizer-facing backend, including:

- Full **tournament CRUD** implementation  
- Registration open/close logic  
- Role-restricted editing (Organizer/Admin only)  
- Integration of Tournament and TournamentPlayer models into the workflow  

**Sprint tasks completed:**
- Implement tournament pages  
- Add feature to open/close registration  
- Restrict tournament editing to its organizer or admin  

---

### Bracket Generation & Match Flow
This was the most technically complex part of the system, and I owned it end-to-end.

I designed and implemented:

- **BracketService** for generating single-elimination brackets  
- Player seeding logic  
- Automatic match creation for each round  
- Winner advancement logic  
- Automatic tournament closure when the final match completes  
- The **bracket display view**, including the layout logic that draws the bracket structure  

**Sprint tasks completed:**
- Implement player seeding and generate bracket structure  
- Implement BracketService for generating and updating brackets  
- Build bracket display page  
- Create match result submission form for referees  
- Restrict editing to assigned referee  
- Automatically advance winner and close tournament  

---

### ELO Rating System
I implemented the entire ELO subsystem, including:

- **ELOService** using the standard ELO formula  
- Automatic rating updates when match results are submitted  
- ELO history tracking per player  

**Sprint tasks completed:**
- Create ELOService  
- Update ELO automatically when match results are recorded  
- Store ELO history per player  

---

### Player Registration Logic
While another teammate handled the UI, I implemented the backend logic that ensures tournament registration behaves correctly.

**Sprint tasks completed:**
- Link registered players to tournaments through TournamentPlayer  
- Validate that a player cannot join a closed tournament  
- Validate that a player cannot register twice  

---

## Summary of Impact
Across the project, I was responsible for the **core competitive logic** that transforms a simple CRUD application into a functioning tournament system:

- Tournament lifecycle  
- Bracket generation  
- Match progression  
- ELO rating updates  
- Backend validation and role-restricted actions  

These systems form the backbone of the application and required careful design, data modeling, and multi-step workflow logic.

---

# Core Features

## User Roles
The system supports multiple roles with distinct permissions:

| Role | Capabilities |
|------|--------------|
| **Administrator** | Manage users, assign roles |
| **Organizer** | Create tournaments, manage registration, seed brackets |
| **Referee** | Record match results, advance brackets |
| **Player** | Join tournaments, view stats and ELO |
| **Public User** | View results, brackets, and leaderboards |

---

# Example User Flow
1. Administrator approves or creates user accounts  
2. Organizer creates a tournament and opens registration  
3. Players join and are seeded into brackets  
4. Referees enter match results  
5. Brackets advance automatically and ELO updates  
6. Public pages display results and leaderboards  

---

# User Stories & Implementation Details

## Authentication & User Management
**User Stories**
- Users can register and log in  
- Admins manage users and roles  

**Implementation**
- ASP.NET Identity with role-based access  
- Seeded roles: Admin, Organizer, Referee, Player  
- Registration/login pages  
- Admin panel for role assignment  
- User profile model with ELO and bio  
- Profile view/edit pages  

---

## Tournament Creation & Management (Organizer)
**User Stories**
- Organizers create tournaments and manage registration  

**Implementation**
- Tournament model (Name, Date, Format, OrganizerId, Status)  
- CRUD operations  
- Registration open/close controls  
- Player seeding and bracket generation  
- Role-restricted editing  

---

## Player Registration & Participation
**User Stories**
- Players join tournaments and view their stats  

**Implementation**
- Registration button for open tournaments  
- TournamentPlayer join table  
- Player dashboard with ELO and history  
- Validation for duplicate or closed registrations  

---

## Match Scheduling & Reporting
**User Stories**
- Brackets auto-generate  
- Referees record match results  
- Players view brackets  

**Implementation**
- Match model (TournamentId, PlayerA, PlayerB, RefereeId, WinnerId, Round)  
- BracketService for generation and updates  
- Bracket display (tree or round-based)  
- Referee-only result submission  
- Automatic advancement and tournament closure  

---

## ELO Ranking System
**User Stories**
- Players see accurate ELO  
- Public users view leaderboards  

**Implementation**
- ELOService using standard ELO formula  
- Automatic rating updates on match submission  
- ELO history tracking  
- Leaderboard page sorted by rating  

---

## Public-Facing Pages
**User Stories**
- Visitors view tournaments and top players  

**Implementation**
- Home page with recent winners and top players  
- Public tournament listing  
- Tournament details with full bracket  
- Optional “About” page  

---

# Tech Stack
- **ASP.NET Core MVC**  
- **Entity Framework Core**  
- **ASP.NET Identity**  
- **SQL Server / LocalDB**  
- **Bootstrap (or chosen CSS framework)**  

---

# About This Portfolio Edition
This repository is a **clean, public-facing reconstruction** of a collaborative academic project.  
To respect collaborator privacy:

- Commit history has been intentionally omitted  
- Identifying information has been removed from comments and documentation  
- All visible code reflects my own contributions  

The original private repository remains intact for verification if needed.

---

# Original Proposed Future Project Enhancements
- Double-elimination bracket support  
- Team-based tournaments  
- Match scheduling calendar  
- API endpoints for mobile clients  
- Real-time bracket updates via SignalR  
