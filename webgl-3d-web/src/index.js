import React from 'react';
import ReactDOM from 'react-dom'

import { Provider } from "mobx-react";
import DataStore from "./state-management/store";
import App from "./App";
import ErrorBoundary from './Components/ErrorBoundary';

const store = new DataStore();

ReactDOM.render(
  <Provider store={store}>
    <ErrorBoundary>
      <App />
    </ErrorBoundary>
  </Provider>,
  document.getElementById("root")
);
