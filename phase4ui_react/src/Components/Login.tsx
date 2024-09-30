// Components/Login.tsx
import React, { useState } from 'react';
import { TextField, Button, Typography, Container } from '@mui/material';

const Login: React.FC<{ toggleDarkMode: () => void; darkMode: boolean, setLoggedIn:(x:boolean)=>void }> = ({ toggleDarkMode, darkMode, setLoggedIn }) => {
  const [email, setEmail] = useState<string>('');
  const [password, setPassword] = useState<string>('');
  const [error, setError] = useState<string>('');

  const handleLogin = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const response = await fetch('https://localhost:7252/Phase4/Reactlogin', { // Adjust URL as needed
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ userEmail: email, password: password }),
    });

    if (response.ok) {
      const data = await response.json();
     
      // Store the token in local storage or context
      localStorage.setItem('token', data.token);
      setError('');
      // Redirect or perform other actions after successful login
      setLoggedIn(true)
      console.log('Login successful!', data.token);
    } else {
      const errorData = await response.json();
      setError('Invalid credentials. Please try again.');
    }
  };

  return (
    <Container component="main" maxWidth="xs">
      <Typography component="h1" variant="h5">
        Login
      </Typography>
      <form onSubmit={handleLogin} noValidate>
        <TextField
          variant="outlined"
          margin="normal"
          required
          fullWidth
          label="Email Address"
          autoComplete="email"
          autoFocus
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
        <TextField
          variant="outlined"
          margin="normal"
          required
          fullWidth
          label="Password"
          type="password"
          autoComplete="current-password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        {error && <Typography color="error">{error}</Typography>}
        <Button type="submit" fullWidth variant="contained" color="primary">
          Login
        </Button>
        <Button onClick={toggleDarkMode} fullWidth>
          Toggle Dark Mode
        </Button>
      </form>
    </Container>
  );
};

export default Login;
