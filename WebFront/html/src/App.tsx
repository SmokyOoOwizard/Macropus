import React from 'react';
import './App.css';
import {BrowserRouter, Route, Routes} from "react-router-dom";
import LoginPage from "./Pages/LoginPage/LoginPage";
import LoginChecker from "./components/loginChecker/LoginChecker";
import PageSidebarWrapper from "./components/pageSidebarWrapper/PageSidebarWrapper";


function App() {
    return (
        <div className="App">
            <BrowserRouter>
                <LoginChecker LoginPage={<LoginPage/>}>
                    <PageSidebarWrapper barEnable={true}>
                        <Routes>
                            <Route path="*" element={<h2>@@@@</h2>}/>
                        </Routes>
                    </PageSidebarWrapper>
                </LoginChecker>
            </BrowserRouter>
        </div>
    );
}

export default App;
