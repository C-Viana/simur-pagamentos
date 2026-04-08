import React from 'react'
import {BrowserRouter, Route, Switch} from 'react-router-dom'
import Login from './pages/Login'
import Payment from './pages/Payment'

export default function Routes() {
    return (
        <BrowserRouter>
            <Switch>
                <Route path='/' exact Component={Login}/>
                <Payment path='/payment' Component={Payment}/>
            </Switch>
        </BrowserRouter>
    )
}