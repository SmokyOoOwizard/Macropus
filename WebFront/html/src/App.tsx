import React from 'react';
import {BrowserRouter, Route, Routes} from "react-router-dom";
import LoginPage from "./Pages/LoginPage/LoginPage";
import LoginChecker from "./components/loginChecker/LoginChecker";
import PageSidebarWrapper from "./components/pageSidebarWrapper/PageSidebarWrapper";
import PageTopbarWrapper from "./components/pageTopbarWrapper/PageTopbarWrapper";


function App() {
    return (
        <div className="App">
            <BrowserRouter>
                <LoginChecker LoginPage={<LoginPage/>}>
                    <PageSidebarWrapper barEnable={true}>
                        <PageTopbarWrapper>
                            <Routes>
                                <Route path="*" element={<h2>@@@@</h2>}/>
                            </Routes>
                        </PageTopbarWrapper>
                    </PageSidebarWrapper>
                </LoginChecker>
            </BrowserRouter>
        </div>
    );
}

export default App;
