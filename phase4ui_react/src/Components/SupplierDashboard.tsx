import React, { useEffect, useState } from 'react';
import { Card, CardContent, Typography, Grid } from '@mui/material';
import ContactPhoneIcon from '@mui/icons-material/ContactPhone';
import BusinessIcon from '@mui/icons-material/Business';
import HomeIcon from '@mui/icons-material/Home';
import EmailIcon from '@mui/icons-material/Email';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import CancelIcon from '@mui/icons-material/Cancel';
import { jwtDecode } from 'jwt-decode';
import SupplierPackageCounts from './SupplierPackageCounts'; // Import the new component

// Define a type for the supplier
interface Supplier {
    supplierID: number;
    supplierName: string;
    supplierContact: number;
    companyName: string;
    companyAddress: string;
    supplierEmail: string;
    type: string;
    registerationStatus: boolean;
}

// Define a type for the decoded token
interface DecodedToken {
    sub: string;    // Email of the user
    type: string;   // Type of the user (supplier, user, admin)
    id: string;     // User ID
    jti: string;    // Unique token identifier
    exp: number;    // Expiration time as Unix timestamp
}

const SupplierDashboard: React.FC = () => {
    const [supplier, setSupplier] = useState<Supplier | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [supplierId, setSupplierId] = useState<number | null>(null);

    const fetchSupplierData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) {
                throw new Error('No token found');
            }

            const decoded: DecodedToken = jwtDecode(token);
            setSupplierId(Number(decoded.id)); // Save supplierId from decoded token
            const response = await fetch(`https://localhost:7252/Phase4/suppliersinfo/${decoded.id}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            const data: Supplier = await response.json();
            setSupplier(data);
        } catch (error: any) {
            setError(error.message);
        }
    };

    useEffect(() => {
        fetchSupplierData();
    }, []);

    return (
        <Grid container spacing={2} style={{ padding: '20px' }}>
            <Grid item xs={12} sm={6}>
                {supplier ? (
                    <Card 
                        variant="outlined" 
                        sx={{ 
                            width: '100%', 
                            height: '100%', 
                            bgcolor: 'background.paper',
                            '&:hover': {
                                transform: 'scale(1.05)', // Scale up on hover
                                boxShadow: '0 4px 20px rgba(255, 255, 255, 0.3)', // Shadow on hover
                            },
                        }}
                    >
                        <CardContent>
                            <Typography variant="h5" component="div">
                                {supplier.supplierName}
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                                <ContactPhoneIcon /> {supplier.supplierContact}
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                                <BusinessIcon /> {supplier.companyName}
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                                <HomeIcon /> {supplier.companyAddress}
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                                <EmailIcon /> {supplier.supplierEmail}
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                                {supplier.registerationStatus ? (
                                    <span>
                                        <CheckCircleIcon color="success" /> Registered
                                    </span>
                                ) : (
                                    <span>
                                        <CancelIcon color="error" /> Not Registered
                                    </span>
                                )}
                            </Typography>
                        </CardContent>
                    </Card>
                ) : (
                    <div>No supplier found.</div>
                )}
            </Grid>
            <Grid item xs={12} sm={6}>
                {supplierId && <SupplierPackageCounts supplierId={supplierId} />} {/* Use the new component */}
            </Grid>
        </Grid>
    );
};

export default SupplierDashboard;
 