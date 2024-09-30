import React, { useState } from 'react';
import { AppBar, Toolbar, Typography, IconButton, Menu, MenuItem, Breadcrumbs, Switch, Button } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';

// Types
type adminHeader = {
  label: string;
  onClick: () => JSX.Element;
};

type headerProp = {
  menuitems: adminHeader[];
  setActiveContent: (ele: React.ReactElement) => void;
  toggleDarkMode: () => void;
  darkMode: boolean;
  setLoggedIn: (x: boolean) => void;
};

const Header: React.FC<headerProp> = ({ menuitems, setActiveContent, toggleDarkMode, darkMode, setLoggedIn }) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [pannelName, setPannelName] = useState<string>('Dashboard');

  const handleMenuClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleMenuItemClick = (onClick: () => JSX.Element, item: adminHeader) => {
    setActiveContent(onClick());
    setPannelName(item.label);
    handleMenuClose();
  };

  const handleLogout = () => {
    // Clear the JWT token from localStorage
    localStorage.removeItem('token');
    // Update the login status
    setLoggedIn(false);
    handleMenuClose();
  };

  return (
    <AppBar position="static" color="primary">
      <Toolbar>
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          Tafri Holidays - {pannelName}
        </Typography>

        {/* Dark/Light Mode Toggle */}
        <Switch
          checked={darkMode}
          onChange={toggleDarkMode}
          name="darkModeToggle"
          inputProps={{ 'aria-label': 'dark mode toggle' }}
        />

        {/* Breadcrumbs with Dropdown Menu */}
        <Breadcrumbs aria-label="breadcrumb">
          <IconButton
            size="large"
            edge="start"
            color="inherit"
            aria-label="menu"
            sx={{ mr: 2 }}
            onClick={handleMenuClick}
          >
            <MenuIcon />
          </IconButton>
        </Breadcrumbs>

        {/* Collapsible Menu with Buttons */}
        <Menu id="menu-appbar" anchorEl={anchorEl} open={Boolean(anchorEl)} onClose={handleMenuClose}>
          {menuitems.map((item, index) => (
            <MenuItem key={index} onClick={() => handleMenuItemClick(item.onClick, item)}>
              {item.label}
            </MenuItem>
          ))}
          {/* Logout Button */}
          <MenuItem onClick={handleLogout}>
            Logout
          </MenuItem>
        </Menu>
      </Toolbar>
    </AppBar>
  );
};

export default Header;
