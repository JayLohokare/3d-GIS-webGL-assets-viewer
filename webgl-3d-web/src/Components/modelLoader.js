import React from 'react';

class ModelLoader extends React.Component {
  state = {
    isLoading: false
  };

  render() {
      if(this.state.isLoading){
        return (
          <div className="progress model-loading-bar black"  >
            <div className="indeterminate"></div>
          </div>
        );
      }
      else{
        return(
          <div></div>
        );
      }
    }
}

export default ModelLoader;