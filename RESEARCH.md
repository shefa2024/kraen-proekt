# Research of Existing Platforms and New Functionality

## 1. Analysis of Existing Platforms
- **Preply**: Focuses on language learning with a strong tutor search and booking system. Uses a subscription model for some tutors.
- **Superprof**: Wide range of subjects beyond languages. Tutors set their own rates. Strong review system.
- **Tutor.com**: More institutional focus, often used by schools/libraries. 24/7 availability for some subjects.

### Strengths & Weaknesses
| Platform | Strengths | Weaknesses |
|----------|-----------|------------|
| Preply | Excellent UI, global reach, language focus | Higher commission for tutors, subscription lock-in |
| Superprof | Diverse subjects, direct tutor contact | Vetting process varies, less automated than others |
| Tutor.com | 24/7 support, institutional trust | Less personal "freelance" feel, more expensive |

## 2. New Functionality for LearnConnect
Based on the analysis, LearnConnect implements the following unique or integrated features:
- **Teacher Rating System**: Students can rate and review teachers after lessons to ensure quality.
- **WebRTC Integration**: Built-in video calling and whiteboard collaboration (via SignalR) for seamless online lessons.
- **Automatic Notifications**: Real-time reminders for upcoming lessons to reduce no-shows.
- **Package Bookings**: Ability for students to book multiple lessons at a discount or in a batch.
- **Gamification**: XP and badges for student progress (implemented via `GamificationService`).

## 3. Technology Selection
- **Backend**: ASP.NET Core 6+ for high performance and scalability.
- **Database**: MS SQL Server with Entity Framework Core (Code First) for robust data management.
- **Frontend**: Single Page Application (SPA) architecture for a modern, responsive user experience.
- **Communication**: SignalR for real-time signaling and collaboration tools.
