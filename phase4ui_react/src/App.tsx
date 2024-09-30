import React, { useState, useEffect } from 'react';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import Supplier from './Components/Supplier';
import User from './Components/User'; // Import the User component
import Admin from './Components/Admin'; // Import the Admin component
import Login from './Components/Login'; // Import the Login component
import {jwtDecode} from 'jwt-decode'; // Import jwt-decode to decode the JWT token
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';

// Define the interface that matches your JWT token structure
interface JwtPayload {
  sub: string;    // Email of the user
  type: string;   // Type of the user (supplier, user, admin)
  id: string;     // User ID
  jti: string;    // Unique token identifier
  exp: number;    // Expiration time as Unix timestamp
}

function App() {
  const [darkMode, setDarkMode] = useState<boolean>(false);
  const [isLoggedIn, setIsLoggedIn] = useState<boolean>(false); // Track login status
  const [userType, setUserType] = useState<string | null>(null); // Store the user type from JWT

  const lightTheme = createTheme({
    palette: {
      mode: 'light',
      primary: { main: '#1976d2' },
      background: { default: '#f5f5f5' },
    },
  });

  const darkTheme = createTheme({
    palette: {
      mode: 'dark',
      primary: {
        main: '#6200ea', // Purple
      },
      background: {
        default: '#121212', // Black
        paper: '#1e1e1e', // Slightly lighter black for cards
      },
      text: {
        primary: '#ffffff', // White text for contrast
        secondary: '#b0bec5', // Light gray for secondary text
      },
    },
  });

  useEffect(() => {
    // Check for JWT token in localStorage
    const token = localStorage.getItem('token');
    console.log(token)
    if (token) {
      try {
        // Decode the token and extract the user type
        const decoded: JwtPayload = jwtDecode(token);
        console.log(decoded)
        // Check if the token is expired
        if (decoded.exp && decoded.exp * 1000 < Date.now()) {
          // Token has expired, remove it from localStorage and set login status to false
          localStorage.removeItem('jwtToken');
          setIsLoggedIn(false);
        } else {
          // Set the login status and user type based on the decoded JWT
          setIsLoggedIn(true);
          setUserType(decoded.type); // Use the 'type' claim to determine the user role
        }
      } catch (error) {
        console.error('Invalid token:', error);
        localStorage.removeItem('jwtToken');
        setIsLoggedIn(false);
      }
    } else {
      // No token, set login status to false
      setIsLoggedIn(false);
    }
  }, [isLoggedIn]);

  return (
    <ThemeProvider theme={darkMode ? darkTheme : lightTheme}>
      <CssBaseline />
      <div className="App">
        {/* Check login status and render the appropriate component based on userType */}
        {!isLoggedIn ? (
          <Login toggleDarkMode={() => setDarkMode(!darkMode)} darkMode={darkMode} setLoggedIn={(x: boolean) => setIsLoggedIn(x)} />
        ) : userType === 'supplier' ? (
          <Supplier toggleDarkMode={() => setDarkMode(!darkMode)} darkMode={darkMode} setLoggedIn={(x: boolean) => setIsLoggedIn(x)} />
        ) : userType === 'user' ? (
          <User toggleDarkMode={() => setDarkMode(!darkMode)} darkMode={darkMode} />
        ) : userType === 'admin' ? (
          <Admin toggleDarkMode={() => setDarkMode(!darkMode)} darkMode={darkMode} />
        ) : (
          <Login toggleDarkMode={() => setDarkMode(!darkMode)} darkMode={darkMode} setLoggedIn={(x: boolean) => setIsLoggedIn(x)} />
        )}
      </div>
    </ThemeProvider>
  );
}

export default App;
