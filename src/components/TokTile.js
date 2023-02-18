import React from 'react'
import '../styles.css'
import Card from "@mui/material/Card";
import CardActions from "@mui/material/CardActions";
import CardContent from "@mui/material/CardContent";
import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import { styled } from "@mui/material/styles";
import CardHeader from '@mui/material/CardHeader';
import Avatar from '@mui/material/Avatar';
import IconButton from '@mui/material/IconButton';
import FavoriteIcon from '@mui/icons-material/Favorite';
import ShareIcon from '@mui/icons-material/Share';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import WaitSpinner from './WaitSpinner';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';
import { red } from '@mui/material/colors';
import Moment from 'moment';
import CardMediaCustom from './CardMediaCustom';
import { getClassToks } from '../services/classServices';

const colors = [
    "4472C7", "732FA0", "05adf4", "73AD46",
    "E23DB5", "BE0400", "195B28", "E88030",
    "873B09", "FFC100"
];

class TokTile extends React.Component {

  // Constructor
	constructor(props) {
		super(props);

		this.state = {
			items: [],
			DataisLoaded: false
		};

    this.reload = this.reload.bind(this);
	}

	// ComponentDidMount is used to
	// execute the code
	componentDidMount() {
    this.getData();
	}
  
  async getData() {
    console.log("api call use effect ");
    const response = await getClassToks(); 
  
    this.setState({
      resultData: response.data,
      continuationToken: response.data.resource.continuationToken,
      items: response.data.resource.results,
      DataisLoaded: true
    });
  }

  reload() {
    this.setState({
      DataisLoaded: false
    });
    this.getData();
  }

  render() {
    const { DataisLoaded, items } = this.state;
    if (!DataisLoaded) return <WaitSpinner />;

    return (
      <Grid container spacing={1}>
        <Grid container spacing={1} >
          <Button onClick={this.reload}>Refresh</Button>
        </Grid>
        {items.map(card => {
            const idx = Math.floor(Math.random() * (colors.length - 1))
            const formattedDate = Moment(card.created_time).format('MM/DD/YY')
            const withImage = (card.image != null ? card.image.length > 1 : false)
            const disp = "none"
            if(withImage) disp = "";
            console.log(disp)

            return (
              <Grid item key={card} xs={12} sm={6} md={4} lg={4}>
                <Card sx={{
                  height: "100%",
                  padding: "8px",
                  margin: "4px",
                  borderRadius: "12px",
                  maxHeight: "326px",
                  minHeight: "326px",
                  display: "flex",
                  flexDirection: "column",
                  border: `4px solid #${colors[idx]}`
                }}>
                  <CardHeader sx={{ textAlign: 'center' }}
                      avatar={
                        <Box sx={{ alignItems: 'center' }}>
                          <Avatar sx={{ border:`2px solid #542a7d` }} src={card.user_photo} aria-label="recipe">
                          </Avatar>
                          <Typography sx={{ fontSize: '11px' }}>{formattedDate}</Typography>
                        </Box>
                      }
                      action={
                          <IconButton aria-label="settings">
                              <MoreVertIcon />
                          </IconButton>
                      }
                      title={card.user_display_name}
                      subheader={card.title_display}
                  />
                  <CardContent>
                      <Typography gutterBottom variant="h6" sx={{ textAlign: "center" }} component="div">
                          {card.primary_text}
                      </Typography>
                      <CardMediaCustom data={card} withImage={withImage} />
                      <Typography variant="body2" color="text.secondary">
                        <List dense="true" sx={{ listStyleType: 'disc', alignContent: 'center' }} >
                          {card.details.map(dtl => {
                            if(dtl == null) return '';
                            if(dtl.length <= 0) return '';

                            return (
                              <ListItem sx={{ paddingTop: '0px', paddingBottom: '0px', textAlign: 'center', display: 'list-item', flexShrink: 0 }}>
                                <ListItemText
                                  primary={dtl}
                                  // secondary={secondary ? 'Secondary text' : null}
                                />
                              </ListItem>
                            )
                          })}
                        </List>
                      </Typography>
                  </CardContent>
                  <CardActions sx={{ display: 'block' }} disableSpacing>
                    <Box
                      component="span"
                      display="flex"
                      justifyContent="space-between"
                      alignItems="center"
                    >
                      <Button  size="small">
                        {card.tok_type}
                      </Button>
                      <Button size="small">
                        {card.category}
                      </Button>
                    </Box>
                  </CardActions>
                </Card>
              </Grid>
            )
          }
        )}
      </Grid>
    );
  }
}

export default TokTile;