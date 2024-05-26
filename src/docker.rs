use crate::cli::GlobalArgs;
use bollard::{errors::Error as BollardError, Docker};

pub fn connect(_args: &GlobalArgs) -> Result<Docker, BollardError> {
	Docker::connect_with_defaults()
}
