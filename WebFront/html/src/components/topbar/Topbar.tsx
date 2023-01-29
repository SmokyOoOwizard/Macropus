import React from "react";
import "./Topbar.scss";
import {FaUser} from "react-icons/fa";

class Topbar extends React.Component {
    render() {
        return (
            <div className="topbar">
                <a className="pageHoverElement" href="2">
                    <FaUser/>
                </a>
                <div className="pageHoverElement">
                    <input type="text" placeholder="search..."/>
                </div>
            </div>
        );
    }
}

export default Topbar;