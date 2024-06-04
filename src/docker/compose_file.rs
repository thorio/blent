//! We deserialize only what we absolutely need, to minimize incompatibilities or errors.

use serde::Deserialize;
use std::collections::HashMap;

#[derive(Debug, Deserialize)]
pub struct ComposeFile {
	pub services: HashMap<String, Service>,
}

#[derive(Debug, Deserialize)]
pub struct Service {
	// Nothing here yet
}
