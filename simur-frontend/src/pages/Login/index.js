import React from 'react';
import './styles.css'

export default function Login(props) {
    return (
        <div className="login-container">
            <h1>SIMUR HOMEPAGE</h1>
            <section className="form">
                <form action="" method="post">
                    <h1>Sign In</h1>
                    
                    <input placeholder='Username'/>
                    <input type='password' placeholder='Password'/>
                    <button className='button' type="submit">Login</button>
                </form>
            </section>
        </div>
    );
}