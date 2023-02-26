import React from 'react'
import CardMedia from "@mui/material/CardMedia";
import CssBaseline from "@mui/material/CssBaseline";
import Toolbar from "@mui/material/Toolbar";
import AppBar from "@mui/material/AppBar";
import Button from "@mui/material/Button";
import CameraIcon from "@mui/icons-material/PhotoCamera";
import Container from "@mui/material/Container";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Typography from "@mui/material/Typography";
import Link from "@mui/material/Link";
import logo from './logo.svg';
import './App.css';
import TokTile from './components/TokTile';

console.log(process.env) // remove this after you've confirmed it is working

function FooterBottomContent() {
  return (
    <Typography variant="body2" color="textSecondary" align="center">
      {"Made by "}
      <Link color="inherit" href="https://tokket.com/">
        Tokket
      </Link>
      {" team."}
    </Typography>
  );
}

function App() {
    return (
      <React.Fragment>
        <CssBaseline />
        <AppBar position="relative">
          <Toolbar>
            <CameraIcon sx={{ marginRight: (theme) => theme.spacing(2) }} />
            <Typography variant="h6" color="inherit" noWrap>
              Tokkepedia
            </Typography>
          </Toolbar>
        </AppBar>
        <main>
          {/* Hero unit */}
          <div sx={{
            backgroundColor: (theme) => theme.palette.background.paper,
            padding: (theme) => theme.spacing(8, 0, 6)
            }}>
            <Container maxWidth="sm">
              <Typography
                component="h1"
                variant="h2"
                align="center"
                color="textPrimary"
                gutterBottom
              >
                TOKKET
              </Typography>
              <Typography
                variant="h5"
                align="center"
                color="textSecondary"
                paragraph
              >
                The Lifetime Learning Platform
              </Typography>
            </Container>
          </div>
          <Container sx={{
            paddingTop: (theme) => theme.spacing(8),
            paddingBottom: (theme) => theme.spacing(8)
            }} maxWidth={false}>
            {/* End hero unit */}
            <TokTile />
          </Container>
        </main>
        {/* Footer */}
        <footer sx={{
          backgroundColor: (theme) => theme.palette.background.paper,
          padding: (theme) => theme.spacing(6)
        }}>
          <Typography variant="h6" align="center" gutterBottom>
            Footer
          </Typography>
          <Typography
            variant="subtitle1"
            align="center"
            color="textSecondary"
            component="p"
          >
            Something here to give the footer a purpose!
          </Typography>
          <FooterBottomContent />
        </footer>
        {/* End footer */}
      </React.Fragment>
  );
}

export default App; 