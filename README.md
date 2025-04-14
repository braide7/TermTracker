# Term Tracker App

A mobile app to help students manage academic terms, courses, and assessments with a clean, intuitive interface. Built with .NET MAUI, this project showcases my full-stack skills in crafting user-focused, cross-platform solutions.

## Table of Contents
- [Introduction](#introduction)
- [Features](#features)
- [Demo](#demo)
- [Installation](#installation)
- [User Guide](#user-guide)
  - [Login or Register](#login-or-register)
  - [Managing Terms](#managing-terms)
  - [Managing Courses](#managing-courses)
  - [Managing Assessments](#managing-assessments)
  - [Reports](#reports)
  - [Troubleshooting](#troubleshooting)
- [Technologies](#technologies)

## Introduction

Term Tracker simplifies academic planning by letting students organize terms, track courses, and manage assessments seamlessly. Developed using .NET MAUI, it reflects my passion for building smooth, engaging apps that solve real problems.

## Features

- Create, edit, and delete terms, courses, and assessments.
- Set notifications for course and assessment start/end dates.
- Share course notes as `.txt` files and export JSON reports of courses.
- Search courses by name or date range with clickable results.
- Secure login with password validation (8+ characters, 1 digit, 1 special character).

## Demo

Download the APK: [term-tracker-app.web.app](https://term-tracker-app.web.app)  
![Term Tracker Demo](screenshots/term-tracker.gif)  
*Note*: Add a demo GIF to `screenshots/` for a visual preview.

## Installation

1. Open a browser on your Android device or emulator and go to [term-tracker-app.web.app](https://term-tracker-app.web.app).
2. Tap the `TermTracker.apk` download link.
3. Open the APK from Downloads or the notification bar.
4. Tap **Install** and wait for completion.
5. Launch the app from your home screen or app drawer.

*Tip*: If installation fails, enable "Install Unknown Apps" in Settings > Apps > Browser.

## User Guide

### Login or Register

- **Register**: Enter a unique username and password (8+ characters, 1 digit, 1 special character). Tap **Register**.
- **Login**: Input your credentials and tap **Login**.
- **Notifications**: Allow notifications on first login for course/assessment alerts.

![login_page](https://github.com/user-attachments/assets/450cd8d8-125f-4662-92f8-aca1ac5b408a)

### Managing Terms

- **View**: Post-login, the **Term List** shows saved terms. A **Logout** button is in the top-right.
- **Add**: Tap **Add New Term**, enter title and start/end dates, then tap **Save**. Choose to stay or return via popup.
- **Edit**: Select a term, tap **Edit** (top-right), update fields, and tap **Save**.
- **Delete**: Select a term, tap **Delete**, and confirm.
- **Courses**: From a term page, tap **Add New Course**.

![launch_home](https://github.com/user-attachments/assets/d93707b1-483b-4882-ba6a-0747eb83b3c4)
![adding_term](https://github.com/user-attachments/assets/2557b2ea-b5e8-47d2-8ddb-07908d393746)


*Note*: A **Home** button on sub-screens returns to the Term List.

### Managing Courses

- **Add**: Tap **Add New Course** from a term page. Enter name, start/end dates (within term range), instructor details, status (dropdown), and optional notifications. Tap **Save**.
- **Notes**: Add notes at the bottom; tap **Share Notes** to export as `.txt`.
- **Edit**: Select a course, tap **Edit**, update fields, and tap **Save**.
- **Delete**: Select a course, tap **Delete**, and confirm.
- **Assessments**: From a course page, tap **Add New Assessment**.

![add_course](https://github.com/user-attachments/assets/03076a34-ca7c-4e35-b2ce-8d5aa612a883)
![added_course](https://github.com/user-attachments/assets/e4d2b4a0-c92b-4100-b0d4-1183c9d8e09b)


### Managing Assessments

- **Add**: Enter name, start/end dates (within course range), type (Objective or Performance), and optional notifications. For Objective, add question count; for Performance, specify task type. Tap **Save**.
- **Edit**: Select an assessment, tap **Edit**, update fields, and tap **Save**.
- **Delete**: Select an assessment, tap **Delete**, and confirm.
  
![add_assessment](https://github.com/user-attachments/assets/e379892d-2a2f-43e5-a357-e2f0d9dc2658)

### Reports

- **Access**: From the Term List (use **Home** if needed), tap **Reports**.
- **Search**: Enter a course name or date range, then tap **Run Report**.
- **Results**: View a table of course name, status, and dates. Tap rows to visit course pages.
- **Export**: Tap **Export Report To File** for a `.txt` file with JSON data and timestamp.
- **Emulator**: Use "Read Aloud" to review exported files.
  
![report_landing](https://github.com/user-attachments/assets/35f81f35-3b50-43e5-b2ee-3bb782048f41)
![report_ran](https://github.com/user-attachments/assets/5faf0406-b51f-48e4-b547-e765a8a2c364)

### Troubleshooting

- **APK Won’t Install**: Enable "Install Unknown Apps" (Settings > Apps > Browser) and re-download from [term-tracker-app.web.app](https://term-tracker-app.web.app).
- **Notifications Fail**: Check Settings > Apps > Term Tracker > Notifications and re-enable permissions.

## Technologies

- .NET MAUI (cross-platform framework)
- C# (logic and services)
- XAML (UI)
- SQLite (local storage)
- Plugin.LocalNotification (alerts)
- Firebase Hosting (APK distribution)

