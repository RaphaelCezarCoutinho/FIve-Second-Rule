//FLOW ENGINE


// Caches all of the flow instances
var flowsInstances = {};

// Custom javascript error for flows
class FlowError extends Error {
	constructor (message, status) {
		super(message);
		this.status = status;
		this.name = 'FlowError';
	}
}

/**
 * A workflow engine which uses a JSON data structure to map and execute the flow of operations, events and conditions
 * @namespace
 * @category Modules
 */

var Flow = class Flow {
	static get ERROR_CODES () {
		return {
			OPERATION_NOT_FOUND: 'OPERATION_NOT_FOUND',
			METHOD_NOT_FOUND: 'METHOD_NOT_FOUND',
			METHOD_NO_RESULT: 'METHOD_NO_RESULT',
			EVENTS_UNDEFINED: 'EVENTS_UNDEFINED',
			EVENT_NOT_FOUND: 'EVENT_NOT_FOUND',
			NO_DISPATCH: 'NO_DISPATCH'
		};
	}

	constructor (flowFile) {
		this._flowFile = flowFile;
		this._flowData = require('../flows/' + flowFile);

		
		this.publicNode = this.findWhere(this._flowData.nodeDataArray, { text: 'public' });
		
	
	
		

		this.publicEvents = this.nextNodesFrom(this.publicNode); // Set public events

		this.globalNode = this.findWhere(this._flowData.nodeDataArray, { text: 'global' });
		this.globalEvents = this.nextNodesFrom(this.globalNode); // Set global events

		console.log(this.globalEvents.length+"  here it is")
	}

	start (callerInstance, source) {

		console.log("here it is again");
		var startNode = this.findWhere(this._flowData.nodeDataArray, { category: 'Start' });

		var nextNodes = this.nextNodesFrom(startNode);

		this.executeNodes(callerInstance, nextNodes, source);
	}

	nextNodesFrom (fromNode, fromPort) {
		var query = { from: fromNode.key };

		if (fromPort) { query.fromPort = fromPort; }

		var nextKeys = this.pluck(this.where(this._flowData.linkDataArray, query), 'to');

		var returnNodes = [];

		nextKeys.forEach((key) => {
			returnNodes.push(this.findWhere(this._flowData.nodeDataArray, { key: key }));
		});

		return returnNodes;
	}

	executeNodes (callerInstance, execNodes, source, data, skipMarker) {
		// If only one link, continue traversing
		if (execNodes.length == 1) {
			var node = execNodes[0];
			var nextNodes;

			switch (node.category) {
				case 'Conditional':

					if (!skipMarker) { callerInstance.setFlowMarker(node.key); }

					var result = this.exec(callerInstance, node.text, source, data);

					var fromPort = result == true ? 'R' : 'B'; // Right port = true, Bottom port = false

					nextNodes = this.nextNodesFrom(node, fromPort);

					this.executeNodes(callerInstance, nextNodes, source, data, skipMarker);

					break;

				case 'End':
					// Don't execute if this is the end node
					break;

				default:

					if (!skipMarker) { callerInstance.setFlowMarker(node.key); }

					this.exec(callerInstance, node.text, source, data);

					// Don't continue if wait message
					if (node.text != 'wait') {
						nextNodes = this.nextNodesFrom(node);

						this.executeNodes(callerInstance, nextNodes, source, data, skipMarker);
					}
					else{
						callerInstance.setFlowMarker(node.key);
					}

					break;
			}
		}
	}

	exec (callerInstance, execText, source, data) {
		// If a method is referenced
		if (execText.length > 0) {
			var execParts = execText.split(':');

			var methodName = execParts[0];
			var methodParams = [];

			// source object is first param to dynamic functions
			methodParams.push(source);

			// Check if params were coded into workflow node and add it to params array,
			// otherwise, if data was passed, add that to params array
			methodParams.push(execParts[1] || data);

			// Get method
			var func = callerInstance[methodName];

			// make sure function exists in model before calling it
			if (func && {}.toString.call(func) === '[object Function]') {
				// Bind the game to the function referenced
				func = func.bind(callerInstance);

				// Call the model's function
				return func.apply(func, methodParams);
			} else {
				throw new FlowError("Method '" + methodName + "' does not exist in referenced model.", Flow.ERROR_CODES.METHOD_NOT_FOUND);
			}
		}
	}

	executePublicEvent (callerInstance, eventName, source, data) {
		// Is this a public event?
		var eventNode = this.findWhere(this.publicEvents, { text: 'event:' + eventName });

		// If this is not a global event, it must be a game flow event
		if (eventNode) {
			var nextNodes = this.nextNodesFrom(eventNode);

			// Execute following nodes and don't move the game marker
			this.executeNodes(callerInstance, nextNodes, source, data, true);
		} else {
			throw new FlowError("Public event '" + eventName + "' does not exist", Flow.ERROR_CODES.EVENT_NOT_FOUND);
		}
	}

	executeEvent (callerInstance, positionKey, eventName, source, data) {
		var skipMarker = true;

		// Is this a public event?
		var eventNode = this.findWhere(this.publicEvents, { text: 'event:' + eventName });

		// If this is not a public event, check global space
		if (!eventNode) {
			eventNode = this.findWhere(this.globalEvents, { text: 'event:' + eventName });
		}

		// If this is also not a global event, it must be a game flow event
		if (!eventNode) {
			// Get starting node at position key
			var startNode = this.findWhere(this._flowData.nodeDataArray, { key: positionKey });

			// Get available next nodes
			var availNextNodes = this.nextNodesFrom(startNode); // Add events from the current flow position

			// Get event node if requested event matches any available ones
			eventNode = this.findWhere(availNextNodes, { text: 'event:' + eventName });
			skipMarker = false;
		}

		if (eventNode) {
			var nextNodes = this.nextNodesFrom(eventNode);

			// Execute following nodes
			this.executeNodes(callerInstance, nextNodes, source, data, skipMarker);
		} else {
			throw new FlowError("Event '" + eventName + "' does not exist at position key " + positionKey, Flow.ERROR_CODES.EVENT_NOT_FOUND);
		}
	}

	isPublicEvent (eventCode) {
		var eventNode = this.findWhere(this.publicEvents, { text: 'event:' + eventCode });

		if (eventNode) { return true; } else { return false; }
	}

	static flowBroker (flowFile) {
		var returnFlow = flowsInstances[flowFile];

		if (!returnFlow) {
			returnFlow = new Flow(flowFile);
			flowsInstances[flowFile] = returnFlow;
		}

		return returnFlow;
	}


	findWhere(list, props) {

		

		
		var match = false;
		var item, prop_k;
		for (let idx=0; idx<list.length; idx++) {
			item = list[idx];
			for (prop_k in props) {
				// If props doesn't own the property, skip it.
				if (!props.hasOwnProperty(prop_k)) continue;
				// If item doesn't have the property, no match;
				if (!item.hasOwnProperty(prop_k)) {
					match = false;
					break;
				}
				if (props[prop_k] === item[prop_k]) {
					// We have a match…so far.
					match = true;
				} else {
					// No match.
					match = false;
					// Don't compare more properties.
					break;
				}
			}
			// We've iterated all of props' properties, and we still match!
			// add that item to the list!
			if (match) {

				return item;
			}
		}
		// No matches
		return null;
	}


	where(list, props){


		let result=[]
		var match = false;
		var item, prop_k;
		for (let idx=0; idx<list.length; idx++) {
			item = list[idx];
			for (prop_k in props) {
				// If props doesn't own the property, skip it.
				if (!props.hasOwnProperty(prop_k)) continue;
				// If item doesn't have the property, no match;
				if (!item.hasOwnProperty(prop_k)) {
					match = false;
					break;
				}
				if (props[prop_k] === item[prop_k]) {
					// We have a match…so far.
					match = true;
				} else {
					// No match.
					match = false;
					// Don't compare more properties.
					break;
				}
			}
			// We've iterated all of props' properties, and we still match!
			// Return that item!
			if (match) {

				result.push(item);
			}
		}
		// No matches
		return result;


	}

	pluck(array, key) {

		
		return array.map(function(obj) {
		  return obj[key];
		});
	}
};
