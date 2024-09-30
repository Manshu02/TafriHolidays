import React from 'react';
import { Card, CardContent, Typography, Button, useTheme } from '@mui/material';

interface PackageCardProps {
    packageData: any;
    toggleActivation: (packageId: string, isActive: boolean, canDeactivate: boolean) => void;
}

const PackageCard: React.FC<PackageCardProps> = ({ packageData, toggleActivation }) => {
    const isPremium = packageData.flightType === 'Premium';
    const isApproved = packageData.isApproved;
    const isActive = packageData.isActive;
    const canDeactivate = packageData.usedCount === 0;

    // Use Material UI's theme hook
    const theme = useTheme();

    return (
        <Card
            sx={{
                maxWidth: 345,
                m: 2,
                backgroundColor: isPremium
                    ? theme.palette.mode === 'dark'
                        ? '#5d5b3d' // Darker shade for premium in dark mode
                        : '#f8f5e1' // Light gold for premium in light mode
                    : theme.palette.mode === 'dark'
                    ? '#3a3a3a' // Darker shade for non-premium in dark mode
                    : '#f2f2f2', // Light silver for non-premium in light mode
                border: `2px solid ${isApproved ? '#39ff14' : 'red'}`, // Neon green if approved, red if not
                transition: 'border-color 0.3s ease', // Smooth transition for border color
            }}
        >
            <CardContent>
                <Typography variant="h5">Package ID: {packageData.packageID || 'Not Included'}</Typography>
                <Typography><strong>Flight Name:</strong> {packageData.flightName || 'Not Included'}</Typography>
                <Typography><strong>From:</strong> {packageData.from || 'Not Included'}</Typography>
                <Typography><strong>To:</strong> {packageData.to || 'Not Included'}</Typography>
                <Typography><strong>Flight Type:</strong> {packageData.flightType || 'Not Included'}</Typography>
                <Typography><strong>Accommodation Name:</strong> {packageData.accName || 'Not Included'}</Typography>
                <Typography><strong>Accommodation Type:</strong> {packageData.accType || 'Not Included'}</Typography>
                <Typography><strong>Total Count:</strong> {packageData.totalCount || 'Not Included'}</Typography>
                <Typography><strong>Used Count:</strong> {packageData.usedCount || '0'}</Typography>
                <Typography><strong>Package Price:</strong> ₹{packageData.packagePrice || 'Not Included'}</Typography>
                <Typography><strong>Is Approved:</strong> {packageData.isApproved ? 'Yes' : 'No'}</Typography>
                <Typography><strong>Is Active:</strong> {packageData.isActive ? 'Yes' : 'No'}</Typography>

                <Button
                    variant="contained"
                    color={isActive ? 'error' : 'success'}
                    disabled={!canDeactivate && isActive}
                    onClick={() => toggleActivation(packageData.packageID, isActive, canDeactivate)}
                >
                    {isActive ? 'Deactivate' : 'Activate'}
                </Button>
            </CardContent>
        </Card>
    );
};

export default PackageCard;
