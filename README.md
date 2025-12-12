# Custom Jeopardy Game

## Overview
Custom Jeopardy is an interactive web-based implementation of the classic game show. Built with modern web technologies, this application allows users to play Jeopardy with customizable categories and questions, featuring an authentic game show experience with scoring, buzzer mechanics, and dynamic question reveals.

## Purpose
This project was developed as part of the coursework at John Abbott College to demonstrate proficiency in front-end web development, JavaScript programming, and interactive UI/UX design. The goal was to create an engaging, fully functional game that captures the essence of the television game show while providing a smooth, responsive user experience.

## Key Features

### Game Mechanics
- **Interactive Question Board**: Grid-based layout with point values ($200 - $1000)
- **Category System**: Customizable categories with themed questions
- **Scoring System**: Real-time score tracking for multiple players
- **Daily Double**: Special questions with betting mechanics
- **Final Jeopardy**: Climactic round with wagering system
- **Buzzer System**: First-to-answer mechanics with lockout functionality

### User Interface
- **Responsive Design**: Works seamlessly on desktop, tablet, and mobile devices
- **Smooth Animations**: Question reveals with flip animations
- **Visual Feedback**: Color-coded correct/incorrect answer indicators
- **Timer Integration**: Countdown timers for answer submission
- **Sound Effects**: Authentic Jeopardy sound effects and music

### Customization
- **Question Editor**: Create your own questions and categories
- **Difficulty Levels**: Adjustable point values
- **Theme Options**: Multiple visual themes to choose from
- **Player Management**: Add/remove players dynamically

## Technologies Used

### Frontend
- **HTML5**: Semantic markup and structure
- **CSS3**: Advanced styling with Flexbox and Grid
- **JavaScript (ES6+)**: Game logic and interactivity
- **DOM Manipulation**: Dynamic content updates

### Design & UX
- **Responsive Layout**: Mobile-first design approach
- **CSS Animations**: Smooth transitions and effects
- **Custom Fonts**: Game show-style typography
- **Color Schemes**: Authentic Jeopardy blue and yellow theme

## How to Play

### Setup
1. Enter player names (1-4 players supported)
2. Select or create a category set
3. Choose difficulty level
4. Start the game!

### Gameplay
1. Player 1 selects a category and point value
2. Question is revealed
3. First player to buzz in gets to answer
4. Correct answers add points, incorrect answers subtract points
5. The player who answered (correctly or not) selects the next question
6. Continue until all questions are answered
7. Proceed to Final Jeopardy with wagering

### Winning
The player with the highest score after Final Jeopardy wins!

## Installation & Setup

### Local Development
```bash
# Clone the repository
git clone https://github.com/BalpreetSingh-code/Custom-Jeopardy.git

# Navigate to project directory
cd Custom-Jeopardy

# Open in browser
# Simply open index.html in your web browser
# Or use a local server:
python -m http.server 8000
# Then visit http://localhost:8000
```

### Deployment
This is a static web application and can be deployed to:
- GitHub Pages
- Netlify
- Vercel
- Any static hosting service

## Project Structure
```
custom-jeopardy/
├── index.html              # Main game interface
├── styles/
│   ├── main.css           # Primary styles
│   ├── board.css          # Game board styling
│   └── animations.css     # Animation effects
├── scripts/
│   ├── game.js            # Core game logic
│   ├── questions.js       # Question management
│   ├── scoring.js         # Score tracking
│   └── ui.js              # UI interactions
├── assets/
│   ├── sounds/            # Audio files
│   ├── images/            # Graphics and icons
│   └── fonts/             # Custom fonts
└── data/
    └── questions.json     # Question database
```

## Code Highlights

### Question Management
The game uses a JSON-based question system for easy customization:
```javascript
{
  "categories": [
    {
      "name": "Computer Science",
      "questions": [
        {
          "value": 200,
          "question": "This language is known for its use in AI and machine learning",
          "answer": "What is Python?",
          "dailyDouble": false
        }
      ]
    }
  ]
}
```

### Scoring Algorithm
Implements accurate scoring with negative point handling:
- Correct answers: Add question value to score
- Incorrect answers: Subtract question value from score
- Daily Doubles: Multiply by wager amount
- Final Jeopardy: Score = (Current Score) ± (Wager)

## Learning Outcomes

Through this project, I gained hands-on experience with:

### Technical Skills
- **DOM Manipulation**: Creating and updating HTML elements dynamically
- **Event Handling**: Managing user interactions and game state
- **State Management**: Tracking game progress without a framework
- **Data Structures**: Efficient organization of questions and player data
- **Asynchronous Programming**: Timers and delayed events

### Design Skills
- **Responsive Web Design**: Creating layouts that work on all devices
- **UI/UX Principles**: Designing intuitive game interfaces
- **Animation Timing**: Creating smooth, professional transitions
- **Color Theory**: Using colors to enhance user experience
- **Accessibility**: Making the game usable for all players

### Problem-Solving
- **Game Logic Implementation**: Translating game rules into code
- **Edge Case Handling**: Managing unexpected user behaviors
- **Performance Optimization**: Ensuring smooth gameplay
- **Cross-Browser Compatibility**: Testing on multiple browsers

## Features Breakdown

### Question Board
- Dynamic grid generation based on category count
- Hover effects and visual feedback
- Question state tracking (answered/unanswered)
- Responsive sizing for different screen sizes

### Buzzer System
- First-to-press detection
- Visual and audio feedback
- Lockout period after incorrect answers
- Timer for answer submission

### Scoring Display
- Real-time score updates
- Animated score changes
- Leader indicator
- Color-coded player scores

### Daily Double
- Random placement algorithm
- Wagering interface with limits
- Special audio cues
- Higher stakes gameplay

## Future Enhancements

### Planned Features
- **Multiplayer Online**: Play with friends remotely
- **Question Database**: Extensive built-in question library
- **Statistics Tracking**: Player performance analytics
- **Tournament Mode**: Bracket-style competitions
- **Custom Themes**: More visual customization options
- **Voice Recognition**: Speak your answers
- **Difficulty Levels**: Easy, Medium, Hard question sets
- **Time Attack Mode**: Speed-based gameplay variant

### Technical Improvements
- Migration to React for better state management
- Backend integration for persistent data
- User authentication and profiles
- Leaderboard system
- Mobile app version using React Native

## Challenges & Solutions

### Challenge 1: Buzzer Timing
**Problem**: Multiple players pressing buzzers simultaneously
**Solution**: Implemented timestamp-based detection with microsecond precision

### Challenge 2: State Management
**Problem**: Keeping track of complex game state without a framework
**Solution**: Created a centralized state object with clear update patterns

### Challenge 3: Responsive Design
**Problem**: Game board becoming too small on mobile devices
**Solution**: Implemented dynamic font sizing and alternative mobile layout

## Testing

The game has been tested on:
- Chrome (Windows, macOS, Android)
- Firefox (Windows, macOS)
- Safari (macOS, iOS)
- Edge (Windows)

Testing included:
- Functionality testing for all game features
- Responsive design testing on various screen sizes
- User experience testing with multiple players
- Performance testing for smooth animations

## Credits & Acknowledgments

### Inspiration
- Jeopardy! - The classic television game show
- Various online Jeopardy implementations

### Resources
- Font Awesome for icons
- Google Fonts for typography
- Sound effects from public domain sources

### Development
Developed as part of the Web Programming curriculum at John Abbott College. This project demonstrates practical application of HTML, CSS, and JavaScript in creating an engaging, interactive web application.

## Screenshots

*Add screenshots here showing:*
- Main game board
- Question reveal
- Daily Double screen
- Final Jeopardy
- Score display
- Mobile responsive view

## Browser Support
- Chrome/Edge: Latest 2 versions
- Firefox: Latest 2 versions
- Safari: Latest 2 versions
- Mobile browsers: iOS Safari 12+, Chrome Mobile

## License
This project is for educational purposes. Jeopardy! is a registered trademark of Jeopardy Productions, Inc.

## Contact
**Developer**: Balpreet Singh Sahota  
**Institution**: John Abbott College - Computer Science  
**GitHub**: [@BalpreetSingh-code](https://github.com/BalpreetSingh-code)  
**Email**: sahotabalpreetsingh1@gmail.com

---

*Built with ❤️ and JavaScript at John Abbott College*
