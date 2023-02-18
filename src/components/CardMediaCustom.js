import * as React from 'react';
import CardMedia from '@mui/material/CardMedia';

export default function CardMediaCustom(props) {
    // console.log(props)
    if(props.withImage) {
        return (
          <CardMedia 
              component="img"
              height="194"
              data-src={props.data.image}
              alt={props.data.primary_text}
              />
        );
    }
    else return ("");
}