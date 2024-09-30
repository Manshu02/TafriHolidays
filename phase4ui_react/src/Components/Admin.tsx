import React from 'react'

type AdminProps = {
    toggleDarkMode: () => void;
    darkMode: boolean;
  };
const Admin:React.FC<AdminProps> = ({ toggleDarkMode, darkMode }) => {
  return (
    <div>
      Admin Dashboard
    </div>
  )
}

export default Admin
