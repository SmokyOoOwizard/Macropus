import React from 'react';
import './App.css';
import {BrowserRouter, Route, Routes} from "react-router-dom";
import LoginPage from "./Pages/LoginPage/LoginPage";
import LoginChecker from "./components/loginChecker/LoginChecker";


function App() {
    return (
        <div className="App">
            <BrowserRouter>
                <LoginChecker LoginPage={<LoginPage/>}>
                    <Routes>
                        <Route path="*" element={<h2>@@@@</h2>}/>
                    </Routes>
                </LoginChecker>
            </BrowserRouter>
        </div>
    );
}

export default App;
