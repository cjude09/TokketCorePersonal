import * as React from 'react';
import CircularProgress from '@mui/material/CircularProgress';
import Box from '@mui/material/Box';

export default function WaitSpinner() {
  return (
    <Box sx={{ textAlign: 'center' }}>
        <CircularProgress />
    </Box> 
  );
}