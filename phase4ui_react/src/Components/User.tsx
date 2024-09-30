import React from 'react'

type UserProps = {
    toggleDarkMode: () => void;
    darkMode: boolean;
  };
const User:React.FC<UserProps> = ({ toggleDarkMode, darkMode }) => {
  return (
    <div>
      User Dashboard
    </div>
  )
}

export default User
