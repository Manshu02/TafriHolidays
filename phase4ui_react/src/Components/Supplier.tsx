import React, { useState } from 'react';
import Header from './Header';
import SupplierDashboard from './SupplierDashboard';
import Addpackage from './Addpackage';
import SupplierPackages from './SupplierPackages';

type adminHeader = {
  label: string;
  onClick: () => JSX.Element;
};

type SupplierProps = {
  toggleDarkMode: () => void;
  darkMode: boolean;
  setLoggedIn:(x:boolean)=>void
};

const Supplier: React.FC<SupplierProps> = ({ toggleDarkMode, darkMode, setLoggedIn }) => {
  const [activeContent, setActiveContent] = useState<React.ReactElement | null>(
    <SupplierDashboard />
  );

  var menuItems: adminHeader[] = [
    { label: 'Dashboard', onClick: () => <SupplierDashboard /> },
    { label: 'Add Package', onClick: () => <Addpackage /> },
    { label: 'Your Packages', onClick:() => <SupplierPackages/>}
  ];

  return (
    <div>
      <Header
        menuitems={menuItems}
        setActiveContent={setActiveContent}
        toggleDarkMode={toggleDarkMode}
        darkMode={darkMode}
        setLoggedIn={setLoggedIn}
      />
      {activeContent}
    </div>
  );
};

export default Supplier;
