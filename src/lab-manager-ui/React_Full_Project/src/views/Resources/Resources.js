import React, { Component } from 'react';
import { Redirect  } from 'react-router-dom';
import {
  Badge,
  Row,
  Col,
  Card,
  CardHeader,
  CardBody,
  Table,
  Button
} from 'reactstrap';


class Resources extends Component {
  render() {
    var data = [];
    fetch('http://localhost:5000/api/resource')
      .then(result => result.json())
      .then(items => data = items);

    var rows = data.map(function (row) {
      return <tr>
        <td>{row.friendlyName}</td>
        <td>{row.ipAddress}</td>
        <td>{row.status}</td>
      </tr>
    });
    return (
      <div className="animated fadeIn">
        <Row>
          <Col>
            <Card>
              <CardHeader>
                <i className="fa fa-align-justify"></i> Resources
              </CardHeader>
              <div>
                <span className="input-group-btn">
                  <Button float="right" outline color="primary" onClick={() => this.redirectToCreateNewResource()}>
                    <i className="fa fa-plus-square-o" ></i>&nbsp; Add New Resource</Button>
                </span>
              </div>

              <CardBody>
                <Table hover bordered striped responsive size="sm">
                  <thead>
                    <tr>
                      <th>Friendly Name</th>
                      <th>IP Address</th>
                      <th>Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {rows}
                  </tbody>
                </Table>
              </CardBody>
            </Card>
          </Col>
        </Row>
      </div >

    )
  }

  redirectToCreateNewResource() {
    <Redirect >alert("Should redirect to create resource");//this.context.router.history.push('/#/resources/create');
    hashHistory.push('/resources/create');
  }
}

export default Resources;
