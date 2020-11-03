import React from 'react';

const errorStyle = {
    zIndex: '99999',
    position: 'absolute',
    top: '50%',
    // left: '3%',
}

class Error extends React.Component {
    render() {
        return (
            <div style={errorStyle} className="center" >
              <h6>API Error. Backend not reachable</h6>
            </div>
        );
    }
}

export default Error;