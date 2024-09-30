import React, { useState, useEffect } from 'react';
import {
  TextField,
  Button,
  Checkbox,
  FormControlLabel,
  Grid,
  Select,
  MenuItem,
  InputLabel,
  FormControl,
  Box,
  Typography,
} from '@mui/material';
import { format } from 'date-fns';
import { SelectChangeEvent } from '@mui/material';
import { jwtDecode } from 'jwt-decode'; // Make sure to install this package

interface PackageFormState {
  TotalCount: number; // Number
  PackagePrice: number; // Number
  Dated: string; // Date as string
  FlightName: string; // String
  From: string; // String
  To: string; // String
  FlightType: string; // String
  AccName: string; // String
  AccType: string; // String
  AccAddress: string; // String
  SSNames: string; // String
  SupplierID: number; // Number
  Luxuary: boolean; // Boolean
}

const Addpackage: React.FC = () => {
  const [formState, setFormState] = useState<PackageFormState>({
    TotalCount: 0,
    PackagePrice: 0,
    Dated: format(new Date(), 'yyyy-MM-dd'),
    FlightName: '',
    From: '',
    To: '',
    FlightType: 'Economy',
    AccName: '',
    AccType: 'Economy',
    AccAddress: '',
    SSNames: '',
    SupplierID: 0, // Initialized as 0
    Luxuary: false,
  });

  const cities = ['Mumbai', 'Delhi', 'Bengaluru', 'Kolkata', 'Chennai'];

  useEffect(() => {
    // Retrieve and decode JWT token
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken: any = jwtDecode(token);
      const supplierId = Number(decodedToken.id); // Ensure this matches the property in your JWT payload
      setFormState(prevState => ({ ...prevState, SupplierID: supplierId }));
    }
  }, []);

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = event.target;
    setFormState(prevState => ({ ...prevState, [name]: value }));
  };

  const handleSelectChange = (event: SelectChangeEvent<string>) => {
    const { name, value } = event.target;
    setFormState(prevState => ({
      ...prevState,
      [name]: value,
    }));
  };

  const handleDateChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { value } = event.target;
    setFormState(prevState => ({
      ...prevState,
      Dated: value,
    }));
  };

  const handleLuxuryToggle = () => {
    setFormState(prevState => ({
      ...prevState,
      Luxuary: !prevState.Luxuary,
      FlightType: !prevState.Luxuary ? 'Premium' : 'Economy',
      AccType: !prevState.Luxuary ? 'Premium' : 'Economy',
      PackagePrice: !prevState.Luxuary ? prevState.PackagePrice * 1.5 : prevState.PackagePrice / 1.5,
    }));
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    try {
      const token = localStorage.getItem('token'); // Retrieve the token from local storage
      const response = await fetch('https://localhost:7252/Phase4/setpackage', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formState),
      });

      const result = await response.text();
      if (result === 'Package registered') {
        alert('Package registered successfully!');
      } else {
        alert('Error registering package: ' + result);
      }
    } catch (error) {
      console.error('Error:', error);
    }
  };

  return (
    <Box sx={{ padding: 4 }}>
      <Typography variant="h4" gutterBottom>
        Add Package
      </Typography>
      <form onSubmit={handleSubmit}>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6}>
            <TextField
              label="Total Count"
              type="number"
              name="TotalCount"
              value={formState.TotalCount}
              onChange={handleInputChange}
              fullWidth
              required
              inputProps={{ max: 50 }}
            />
          </Grid>

          <Grid item xs={12} sm={6}>
            <TextField
              label="Package Price"
              type="number"
              name="PackagePrice"
              value={formState.PackagePrice}
              onChange={handleInputChange}
              fullWidth
              required
              inputProps={{ max: 50000 }}
            />
          </Grid>

          <Grid item xs={12} sm={6}>
            <TextField
              label="Package Date"
              type="date"
              value={formState.Dated}
              onChange={handleDateChange}
              fullWidth
              required
              InputLabelProps={{
                shrink: true,
              }}
            />
          </Grid>

          <Grid item xs={12}>
            <FormControlLabel
              control={<Checkbox checked={formState.Luxuary} onChange={handleLuxuryToggle} />}
              label="Enable Luxury"
            />
          </Grid>

          <Grid item xs={12}>
            <FormControlLabel
              control={
                <Checkbox
                  checked={!!formState.FlightName}
                  onChange={() => setFormState(prev => ({ ...prev, FlightName: formState.FlightName ? '' : ' ' }))}
                />
              }
              label="Include Flight Details"
            />
          </Grid>

          {formState.FlightName && (
            <>
              <Grid item xs={12} sm={6}>
                <TextField
                  label="Flight Name"
                  name="FlightName"
                  value={formState.FlightName}
                  onChange={handleInputChange}
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth>
                  <InputLabel id="from-label">From</InputLabel>
                  <Select
                    labelId="from-label"
                    name="From"
                    value={formState.From}
                    onChange={handleSelectChange}
                  >
                    {cities.map(city => (
                      <MenuItem key={city} value={city}>
                        {city}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth>
                  <InputLabel id="to-label">To</InputLabel>
                  <Select
                    labelId="to-label"
                    name="To"
                    value={formState.To}
                    onChange={handleSelectChange}
                  >
                    {cities.filter(city => city !== formState.From).map(city => (
                      <MenuItem key={city} value={city}>
                        {city}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth>
                  <InputLabel id="flight-type-label">Flight Type</InputLabel>
                  <Select
                    labelId="flight-type-label"
                    name="FlightType"
                    value={formState.FlightType}
                    onChange={handleSelectChange}
                  >
                    <MenuItem value="Economy">Economy</MenuItem>
                    <MenuItem value="Premium">Premium</MenuItem>
                  </Select>
                </FormControl>
              </Grid>
            </>
          )}

          <Grid item xs={12}>
            <FormControlLabel
              control={
                <Checkbox
                  checked={!!formState.AccName}
                  onChange={() => setFormState(prev => ({ ...prev, AccName: formState.AccName ? '' : ' ' }))}
                />
              }
              label="Include Accommodation Details"
            />
          </Grid>

          {formState.AccName && (
            <>
              <Grid item xs={12} sm={6}>
                <TextField
                  label="Accommodation Name"
                  name="AccName"
                  value={formState.AccName}
                  onChange={handleInputChange}
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth>
                  <InputLabel id="acc-type-label">Accommodation Type</InputLabel>
                  <Select
                    labelId="acc-type-label"
                    name="AccType"
                    value={formState.AccType}
                    onChange={handleSelectChange}
                  >
                    <MenuItem value="Economy">Economy</MenuItem>
                    <MenuItem value="Premium">Premium</MenuItem>
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12}>
                <TextField
                  label="Accommodation Address"
                  name="AccAddress"
                  value={formState.AccAddress}
                  onChange={handleInputChange}
                  fullWidth
                />
              </Grid>
            </>
          )}

          <Grid item xs={12}>
            <TextField
              label="Special Services Names"
              name="SSNames"
              value={formState.SSNames}
              onChange={handleInputChange}
              fullWidth
            />
          </Grid>
        </Grid>
        <Box mt={3}>
          <Button type="submit" variant="contained" color="primary">
            Submit
          </Button>
        </Box>
      </form>
    </Box>
  );
};

export default Addpackage;
