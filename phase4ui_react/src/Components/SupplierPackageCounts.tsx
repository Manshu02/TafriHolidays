// SupplierPackageCounts.tsx
import React, { useEffect, useState } from 'react';
import { Card, CardContent, Typography, Grid } from '@mui/material';
import { jwtDecode } from 'jwt-decode';

// Define a type for package counts
interface PackageCounts {
    totalPackages: number;
    activePackages: number;
    approvedPackages: number;
}

const SupplierPackageCounts: React.FC<{ supplierId: number }> = ({ supplierId }) => {
    const [packageCounts, setPackageCounts] = useState<PackageCounts | null>(null);
    const [error, setError] = useState<string | null>(null);

    const fetchPackageCounts = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) {
                throw new Error('No token found');
            }

            const response = await fetch(`https://localhost:7252/Phase4/supplier-package-counts/${supplierId}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            const data: PackageCounts = await response.json();
            setPackageCounts(data);
        } catch (error: any) {
            setError(error.message);
        }
    };

    useEffect(() => {
        fetchPackageCounts();
    }, [supplierId]);

    return (
        <div style={{ padding: '20px' }}>
            <h2>Package Counts</h2>
            {error && <div>Error: {error}</div>}
            {packageCounts ? (
                <Grid container spacing={2} justifyContent="center">
                    <Grid item xs={12} sm={4}>
                        <Card variant="outlined">
                            <CardContent>
                                <Typography variant="h5" component="div">
                                    Total Packages: {packageCounts.totalPackages}
                                </Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                    <Grid item xs={12} sm={4}>
                        <Card variant="outlined">
                            <CardContent>
                                <Typography variant="h5" component="div">
                                    Active Packages: {packageCounts.activePackages}
                                </Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                    <Grid item xs={12} sm={4}>
                        <Card variant="outlined">
                            <CardContent>
                                <Typography variant="h5" component="div">
                                    Approved Packages: {packageCounts.approvedPackages}
                                </Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                </Grid>
            ) : (
                <div>No package counts available.</div>
            )}
        </div>
    );
};

export default SupplierPackageCounts;
