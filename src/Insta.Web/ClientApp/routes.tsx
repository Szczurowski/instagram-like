import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Details } from "./components/Details";

export const routes = <Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/photoDetails/:id' component={Details} />
</Layout>;
