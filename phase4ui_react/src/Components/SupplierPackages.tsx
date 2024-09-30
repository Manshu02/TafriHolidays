import React, { useEffect, useState } from 'react';
import {jwtDecode} from 'jwt-decode';
import PackageCard from './PackageCard';
import { Grid, Typography, Container } from '@mui/material';

const PackagesList: React.FC = () => {
    const [packages, setPackages] = useState<any[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    
    // Retrieve the token from localStorage
    const token = localStorage.getItem('token');

    useEffect(() => {
        if (!token) {
            // Handle the case where there's no token (e.g., redirect to login page)
            alert('No token found. Please log in.');
            // You can also redirect to the login page here, e.g., using a history push or window.location
            return;
        }

        try {
            const decodedToken: any = jwtDecode(token); // Only decode if the token exists
            const supplierId = Number(decodedToken.id);

            fetch(`https://localhost:7252/Phase4/getpackages/${supplierId}`, {
                headers: {
                    Authorization: `Bearer ${token}`, // Send token in the header
                    'Content-Type': 'application/json',
                },
            })
                .then(response => response.json())
                .then(data => {
                    setPackages(data);
                    setLoading(false);
                })
                .catch(error => {
                    console.error('Error fetching packages:', error);
                    setLoading(false);
                });
        } catch (error) {
            console.error('Error decoding token:', error);
            alert('Invalid token. Please log in again.');
            // Redirect to login or handle the error appropriately
        }
    }, [token]);

    const toggleActivation = (packageId: string, isActive: boolean, canDeactivate: boolean) => {
        if (!canDeactivate && isActive) {
            alert('Cannot deactivate a package with used count greater than 0.');
            return;
        }

        const action = isActive ? 'deactivate' : 'activate';

        fetch(`https://localhost:7252/Phase4/updatepackage/${packageId}`, {
            method: 'POST',
            headers: {
                Authorization: `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ action }),
        })
            .then(response => {
                if (response.ok) {
                    // Reload the packages after the update
                    setPackages(prevPackages =>
                        prevPackages.map(p =>
                            p.packageID === packageId ? { ...p, isActive: !isActive } : p
                        )
                    );
                } else {
                    throw new Error('Error in updating package status');
                }
            })
            .catch(error => {
                console.error('Error:', error);
            });
    };

    return (
        <Container sx={{ mt: 4 }}>
            <Typography variant="h4" align="center" gutterBottom>
                Total Packages: {packages.length}
            </Typography>
            {loading ? (
                <Typography align="center">Loading packages...</Typography>
            ) : (
                <Grid container spacing={4}>
                    {packages.map(pkg => (
                        <Grid item key={pkg.packageID} xs={12} sm={6} md={4}>
                            <PackageCard
                                packageData={pkg}
                                toggleActivation={toggleActivation}
                            />
                        </Grid>
                    ))}
                </Grid>
            )}
        </Container>
    );
};

export default PackagesList;
